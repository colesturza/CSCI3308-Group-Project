﻿(function () {

    var mdConverter = new showdown.Converter();
    setShowdownDefaults(mdConverter);

    var simplemde;
    var postRawData;
    var jsonPostDataOld = null;
    var oldResponseErr = null;


    Vue.component('comment-component', {
        props: ['comment'],
        template: '<div class="container">' +
            '            <div class="container-fluid">' +
            '                <div>' +
            '                    <span class="m-2 mr-0">Posted by</span>' +
            '                    <a class="">[{{ comment.CreatedBy }}]</a>' +
            '                    <span>{{ comment.CreatedDate.substring(0,10) }}</span>' +
            '                </div>' +
            '                <div class="border border-dark rounded m-2 py-2">' +
            '                    <span class="text-body p-2">' +
            '                        {{ comment.Content }}' +
            '                    </span>' +
            '                </div>' +
            '                <button type="button" class="btn-sm btn-outline-dark m-2 mb-1" v-on:click="emit" data-reply="reply" style="display:none !important">Reply</button>' +
            '                <div>' +
            '                    <div v-bind:data-cmtID="comment.ID" class="form-group" style="display: none;">' +
            '                        <textarea rows="2" class="mx-auto form-control" ref="commentReply"></textarea>' +
            '                        <button v-bind:data-submitID="comment.ID" type="button" class="btn-sm btn-outline-primary m-2 mb-1" v-on:click="submitComment">Submit</button>' +
            '                    </div>' +
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
                    ParentID: this.comment.ID
                };

                var jsonData = JSON.stringify(formData);


                $.ajax({
                    method: "POST",
                    url: "/uhubapi/comments/Create",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: jsonData,
                    success: function (data) {
                        $("[data-cmtID=" + formData.ParentID + "] textarea").val("");
                        $("[data-cmtID=" + formData.ParentID + "]").toggle();

                        var today = new Date();
                        var dd = today.getDate();
                        var mm = today.getMonth() + 1; //January is 0!
                        var yyyy = today.getFullYear();
                        var dtStr = yyyy.toString().padStart(4, '0') + '-' + mm.toString().padStart(2, '0') + '-' + dd.toString().padStart(2, '0');

                        var newCmt = {
                            ID: data,
                            CreatedBy: "me",
                            CreatedDate: dtStr,
                            Content: formData.Content
                        };

                        vueInstance.comments.splice(0, 0, newCmt);
                    },
                    error: function (jqAjax, errorText) {
                        alert("Error" + errorText);
                    }
                });
            }
        }
    });



    new Vue({
        el: "#post-container",
        data: {
            parentID: "",
            title: "",
            content: "",
            postTime: "",
            comments: []
        },
        methods: {
            buttonHandler: function (commentID) {
                $("[data-cmtID=" + commentID + "]").toggle();
            },
            postReply: function () {
                $("#post-reply").toggle();
            },
            submitCommentPost: function () {

                var postId = encodeURIComponent(window.location.href.split('/').slice(-1)[0]);


                var formData = {
                    Content: this.$refs.postReplyText.value,
                    ParentID: postId
                };

                var jsonData = JSON.stringify(formData);

                $.ajax({
                    method: "POST",
                    url: "/uhubapi/comments/Create",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: jsonData,
                    success: function (data) {
                        $("#post-reply textarea").val("");
                        $("#post-reply").toggle();

                        var today = new Date();
                        var dd = today.getDate();
                        var mm = today.getMonth() + 1; //January is 0!
                        var yyyy = today.getFullYear();
                        var dtStr = yyyy.toString().padStart(4, '0') + '-' + mm.toString().padStart(2, '0') + '-' + dd.toString().padStart(2, '0');

                        var newCmt = {
                            ID: data,
                            CreatedBy: "me",
                            CreatedDate: dtStr,
                            Content: formData.Content
                        };

                        vueInstance.comments.splice(0, 0, newCmt);
                    },
                    error: function (jqAjax, errorText) {
                        alert("Error" + errorText);
                    }
                });
            }
        },
        mounted: function () {
            vueInstance = this;
            var self = this;

            var postID = encodeURIComponent(window.location.href.split('/').slice(-1)[0]);
            postID = postID.match(/^[0-9]+/);


            $.ajax({
                method: "POST",
                url: "/uhubapi/posts/GetByID?PostID=" + postID,
                success: function (data) {
                    postRawData = data;

                    var clubID = data.ParentID;

                    self.parentID = data.ParentID;
                    self.title = htmlEncode(data.Name);
                    window.setTimeout(function () {
                        simplemde.value(data.Content);
                        //set old value to prevent clean updates
                        oldResponseErr = 'Nothing to Update';
                        jsonPostDataOld = JSON.stringify(getData());
                    }, 1);
                    self.postTime = data.CreatedDate;


                    $("#post-container").style('display', null);
                    if (data.CanComment) {
                        $("#btn_ToggleReply").style('display', null);
                    }


                    //fetch comments if necessary
                    if (self.title != undefined && self.title != null && self.title != "") {
                        $.ajax({
                            method: "POST",
                            url: "/uhubapi/comments/GetByPost?PostID=" + encodeURIComponent(data.ID),
                            error: function (jqAjax, errorText) {
                                alert("Error" + errorText);
                            },
                            success: function (data) {
                                data.sort(dynamicSort("-CreatedDate"));
                                self.comments = data;

                                if (data.length == 0 || !postRawData.CanComment) {
                                    return;
                                }

                                var cmtInterval = window.setInterval(function () {
                                    var cmtBtnSet = $("[data-reply=reply]");
                                    if (cmtBtnSet.length == 0) {
                                        return;
                                    }
                                    for (var i = 0; i < cmtBtnSet.length; i++) {
                                        $(cmtBtnSet[i]).style('display', null);
                                    }
                                    window.clearInterval(cmtInterval);
                                }, 10);

                            }
                        });
                    }




                    //set navbar title to current club
                    var comms = $("#navbarDropdownMenu a");
                    var commLen = comms.length;
                    for (var i = 0; i < commLen; i++) {
                        if (clubID == $(comms[i]).attr("data-ClubID")) {
                            $("#navbarDropdownMenuLink").text($(comms[i]).text());
                        }
                    }

                },
                error: function (jqAjax, errorText) {
                    alert("Error " + errorText);
                }
            });
        }
    });


    simplemde = new SimpleMDE(
        {
            autosave: {
                enabled: false
            },
            element: document.getElementById("txt_PostArea"),
            previewRender: function (plainText) {
                return mdConverter.makeHtml(plainText);
            },
            promptURLs: true,
            spellChecker: true,
            status: ["lines", "words", {
                className: "charCount",
                defaultValue: function (el) {
                    this.charCount = 0;
                    el.innerHTML = "Characters: 0 (10 - 10k)";
                },
                onUpdate: function (el) {
                    var ct = simplemde.value().length;
                    el.innerHTML = "Characters: " + ct + " (10 - 10k)";
                }
            }]
        });





    function getData() {
        postRawData.Content = simplemde.value();

        return {
            ID: postRawData.ID,
            Name: postRawData.Name,
            Content: postRawData.Content,
            IsPublic: postRawData.IsPublic,
            CanComment: postRawData.CanComment
        };

    }


    
    function sendData(formData) {
        $("#btn_UpdatePost").attr("disabled", "disabled");
        $("html").css({ cursor: "wait" });

        var jsonPostData = JSON.stringify(formData);

        if (jsonPostData == jsonPostDataOld) {
            alert(oldResponseErr);
            $("#btn_CreateUser").removeAttr("disabled");
            $("html").css({ cursor: "default" });
            return;
        }
        jsonPostDataOld = jsonPostData;



        if (!$("#txt_PostArea").val().match(/^.{10,10000}$/)) {
            oldResponseErr = 'Post Content Invalid';
            alert(oldResponseErr);
            $("#btn_UpdatePost").removeAttr("disabled");
            $("html").css({ cursor: "default" });
            return;
        }



        $.ajax({
            method: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonPostData,
            url: "/uhubapi/posts/Update",
            complete: function (data) {
                $("#btn_UpdatePost").removeAttr("disabled");
                $("html").css({ cursor: "default" });
                console.log(data);
            },
            success: function (data) {
                jsonPostDataOld = null;
                alert(data);
            },
            error: function (data) {
                oldResponseErr = data.responseJSON;
                alert(oldResponseErr);
            }
        });
    }


    $("#btn_UpdatePost").click(function () {
        var data = getData();
        sendData(data);
    });




    registerInputValidator($("#txt_PostArea"), /^.{10,10000}$/);



})();
