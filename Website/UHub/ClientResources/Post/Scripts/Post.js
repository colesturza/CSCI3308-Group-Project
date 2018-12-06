(function () {

    var vueInstance;
    var postRawData;
    var rawCommentSet = []


    var postID = encodeURIComponent(window.location.href.split('/').slice(-1)[0]);
    postID = postID.match(/^[0-9]+/)[0];


    var mdConverter = new showdown.Converter();
    setShowdownDefaults(mdConverter);



    function getTodayDateStr() {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();
        return yyyy.toString().padStart(4, '0') + '-' + mm.toString().padStart(2, '0') + '-' + dd.toString().padStart(2, '0');
    }


    function execCreateComment(jsonData) {

        return $.ajax({
            method: "POST",
            url: "/uhubapi/comments/Create",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonData
        })
            .fail(function (jqAjax, errorText) {
                alert("Error" + errorText);
            });
    }



    Vue.component('comment-component', {
        props: ['comment'],
        template: '<div class="container">' +
            '            <div class="container-fluid">' +
            '                <div>' +
            '                    <span class="m-2 mr-0">Posted by</span>' +
            '                    <a class="">[{{ comment.CreatedBy }}]</a>' +
            '                    <span>{{ comment.CreatedDate.toString().substring(0,10) }}</span>' +
            '                </div>' +
            '                <div class="border border-dark rounded m-2 py-2">' +
            '                    <span class="text-body p-2">' +
            '                        {{ comment.Content }}' +
            '                    </span>' +
            '                </div>' +
            '                <button type="button" class="btn-sm btn-outline-dark m-2 mb-1" v-on:click="emit(comment.ID)" data-reply="reply" style="display:none !important">Reply</button>' +
            '                <div>' +
            '                    <div v-bind:data-cmtID="comment.ID" class="form-group" style="display: none;">' +
            '                        <textarea rows="2" class="mx-auto form-control" ref="commentReply"></textarea>' +
            '                        <button v-bind:data-submitID="comment.ID" type="button" class="btn-sm btn-outline-primary m-2 mb-1" v-on:click="submitComment">Submit</button>' +
            '                    </div>' +
            '                </div>' +
            '               <div>' +
            '                   <comment-component v-for="comment in comment.cmt_children" v-on:custom-click="buttonHandler" :key="comment.id" v-bind:comment="comment"><comment-component>' +
            '               </div>' +
            '            </div>' +
            '        </div>',
        methods: {
            emit: function (cmtID) {
                this.$emit('custom-click', cmtID);
            },
            buttonHandler: function (commentID) {
                $("[data-cmtID=" + commentID + "]").toggle();
            },
            submitComment: function () {

                var formData = {
                    Content: this.$refs.commentReply.value,
                    ParentID: this.comment.ID
                };

                var jsonData = JSON.stringify(formData);


                execCreateComment(jsonData)
                    //AJAX -> /uhubapi/comments/Create
                    .done(function (data) {
                        $("[data-cmtID=" + formData.ParentID + "] textarea").val("");
                        $("[data-cmtID=" + formData.ParentID + "]").toggle();


                        var dtStr = getTodayDateStr();

                        var newCmt = {
                            ID: data,
                            CreatedBy: "me",
                            CreatedDate: dtStr,
                            Content: formData.Content
                        };

                        rawCommentSet.splice(0, 0, newCmt);
                        var cmtArrangedList = self.arrangeCommentTree(rawCommentSet);
                        vueInstance.comments = cmtArrangedList;
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


                var formData = {
                    Content: this.$refs.postReplyText.value,
                    ParentID: postID
                };

                var jsonData = JSON.stringify(formData);


                execCreateComment(jsonData)
                    .done(function (data) {
                        $("#post-reply textarea").val("");
                        $("#post-reply").toggle();

                        var dtStr = getTodayDateStr();


                        var newCmt = {
                            ID: data,
                            CreatedBy: "me",
                            CreatedDate: dtStr,
                            Content: formData.Content
                        };


                        rawCommentSet.splice(0, 0, newCmt);
                        var cmtArrangedList = self.arrangeCommentTree(rawCommentSet);
                        vueInstance.comments = cmtArrangedList;
                    })
            },
            // getCommentById and getCommentDepth  are helper functions for arrangeCommentTree
            // Returns matching comment object
            getCommentById: function (cmtID, cmtList) {
                for (var j = 0; j < cmtList.length; j++) {
                    if (cmtList[j].ID == cmtID) {
                        return cmtList[j];
                    }
                }
            },
            // Finds the comment's degrees of separation from post
            getCommentDepth: function (theCmt, cmtList) {

                var depthLevel = 0;
                while (theCmt.ParentID != postID) {
                    theCmt = this.getCommentById(theCmt.ParentID, cmtList);
                    depthLevel++;
                }
                return depthLevel;
            },
            // Arranges array so that children are within parent comments
            arrangeCommentTree: function (cmtList) {
                var maxDepth = 0;
                var listLength = cmtList.length;

                //Remove disabled comments
                for (var i = listLength - 1; i >= 0; i--) {
                    if (!cmtList[i].IsEnabled) {
                        cmtList.splice(i, 1);
                    }
                }

                //Get depth level for each comment
                for (var i = 0; i < listLength; i++) {
                    cmtList[i].cmt_children = [];
                    cmtList[i].DepthLevel = this.getCommentDepth(cmtList[i], cmtList);
                    if (cmtList[i].DepthLevel > maxDepth) {
                        maxDepth = cmtList.DepthLevel;
                    }
                }

                //create hierarchy
                for (var j = 0; j < listLength; j++) {

                    if (cmtList[j].DepthLevel == 0) {
                        continue;
                    }

                    for (var k = 0; k < listLength; k++) {

                        if (cmtList[k].ID == cmtList[j].ParentID) {
                            cmtList[k].cmt_children.push(cmtList[j]);
                            break;
                        }
                    }
                }


                for (var i = (cmtList.length - 1); i >= 0; i--) {
                    if (cmtList[i].DepthLevel != 0) {
                        cmtList.splice(i, 1);
                    }
                }

                return cmtList;
            }
        },
        mounted: function () {
            vueInstance = this;
            var self = this;


            $.ajax({
                method: "POST",
                url: "/uhubapi/posts/GetByID?PostID=" + encodeURIComponent(postID)
            })
                .done(function (data) {
                    postRawData = data;
                    var clubID = data.ParentID;


                    self.parentID = data.ParentID;
                    self.title = htmlEncode(data.Name);
                    self.content = mdConverter.makeHtml(data.Content);
                    self.postTime = data.CreatedDate;


                    $("#post-container").style('display', null);
                    if (data.CanComment) {
                        $("#btn_ToggleReply").style('display', null);
                    }


                    //fetch comments if necessary
                    if (self.title != undefined && self.title != null && self.title != "") {
                        $.ajax({
                            method: "POST",
                            url: "/uhubapi/comments/GetByPost?PostID=" + encodeURIComponent(postID)
                        })
                            .done(function (data) {
                                rawCommentSet = cmtData;
                                var cmtArrangedList = self.arrangeCommentTree(rawCommentSet);
                                //console.log(JSON.parse(JSON.stringify(cmtArrangedList)));

                                self.comments = cmtArrangedList



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


                            })
                            .fail(function (jqAjax, errorText) {
                                alert("Error" + errorText);
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

                })
                .fail(function (jqAjax, errorText) {
                    alert("Error " + errorText);
                });
        }
    });



})();
