﻿(function () {

    var mdConverter = new showdown.Converter();

    Vue.component('comment-component', {
        props: ['comment'],
        template: '<div class="container">' +
            '            <div class="container-fluid">' +
            '                <div>' +
            '                    <span class="m-2 mr-0">Posted by</span>' +
            '                    <a class="">{{ comment.CreatedBy }}</a>' +
            '                    <span>{{ comment.CreatedDate.substring(0,10) }}</span>' +
            '                </div>' +
            '                <div class="border border-dark rounded m-2 py-2">' +
            '                    <span class="text-body p-2">' +
            '                        {{ comment.Content }}' +
            '                    </span>' +
            '                </div>' +
            '                <button type="button" class="btn-sm btn-outline-dark m-2 mb-1" v-on:click="emit">Reply</button>' +
            '                <button v-bind:data-submitID="comment.ID" type="button" class="btn-sm btn-outline-primary m-2 mb-1" v-on:click="submitComment">Submit</button>' +
            '                <div>' +
            '                    <form v-bind:data-cmtID="comment.ID" class="form-group">' +
            '                        <textarea rows="2" class="mx-auto form-control" ref="commentReply"></textarea>' +
            '                    </form>' +
            '                </div>' +
            '            </div>' +
            '        </div>',
        methods: {
            emit: function () {
                this.$emit('custom-click', this.comment.ID);
            },
            submitComment: function () {

                var formData = {
                    Content: this.$refs.commentReply.value,
                    ParentID: this.comment.ParentID
                };

                var jsonData = encodeURIComponent(formData);


                $.ajax({
                    method: "POST",
                    url: "uhubapi/comments/Create",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: jsonData,
                    error: function (jqAjax, errorText) {
                        alert("Error" + errorText);
                    }
                });
            }
        }
    });

    var postDisplay = new Vue({
        el: "#post-container",
        data: {
            title: "",
            content: "",
            postTime: "",
            comments: []
        },
        methods: {
            buttonHandler: function (commentID) {
                $("[data-cmtID=" + commentID + "]").toggle();
                $("[data-submitID=" + commentID + "]").toggle();
            },
            postReply: function () {
                $("#post-reply").toggle();
            },
            submitCommentPost: function () {

                var formData = {
                    Content: this.$refs.postReplyText.value,
                    ParentID: 1
                };

                var jsonData = JSON.stringify(formData);

                $.ajax({
                    method: "POST",
                    url: "uhubapi/comments/Create",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: jsonData,
                    error: function (jqAjax, errorText) {
                        alert("Error" + errorText);
                    }
                });
            }
        },
        beforeMount: function () {
            var postReq = $.ajax({
                method: "POST",
                url: "uhubapi/posts/GetByID?PostID=" + encodeURIComponent(window.location.href.split('/').slice(-1)[0]),
                error: function (jqAjax, errorText) {
                    alert("Error " + errorText);
                },
                statusCode: {
                    200: function () {
                        this.title = postReq.Name;
                        this.content = mdConverter.makeHtml(postReq.Content);
                        this.postTime = postReq.CreatedDate;
                        if (postReq.CanComment && postReq.Name.html().text() != null) {
                            var commentReq = $.ajax({
                                method: "POST",
                                url: "uhubapi/comments/GetByPost?PostID=" + encodeURIComponent(postReq.ID),
                                error: function (jqAjax, errorText) {
                                    alert("Error" + errorText);
                                },
                                success: function () {
                                    this.comments = commentReq.responseJSON;
                                }
                            });
                        }
                    }
                }
            });
        }
    });

})();
