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

    // getCommentById and getCommentDepth  are helper functions for arrangeCommentTree
    // Returns matching comment object
    function getCommentById(cmtID, cmtList) {
        for (var j = 0; j < cmtList.length; j++) {
            if (cmtList[j].ID == cmtID) {
                return cmtList[j];
            }
        }
    }
    // Finds the comment's degrees of separation from post
    function getCommentDepth(theCmt, cmtList) {

        var depthLevel = 0;
        while (theCmt.ParentID != postID) {
            theCmt = getCommentById(theCmt.ParentID, cmtList);
            depthLevel++;
        }
        return depthLevel;
    }
    // Arranges array so that children are within parent comments
    function arrangeCommentTree(cmtList) {
        var maxDepth = 0;
        var newCmtList = cmtList.slice();

        var listLength = newCmtList.length;

        //Remove disabled comments
        for (var i = listLength - 1; i >= 0; i--) {
            if (!newCmtList[i].IsEnabled) {
                newCmtList.splice(i, 1);
            }
        }
        listLength = newCmtList.length;


        //Get depth level for each comment
        for (var i = 0; i < listLength; i++) {
            newCmtList[i].cmt_children = [];
            newCmtList[i].DepthLevel = getCommentDepth(newCmtList[i], newCmtList);
            if (newCmtList[i].DepthLevel > maxDepth) {
                maxDepth = newCmtList.DepthLevel;
            }
        }

        //create hierarchy
        for (var j = 0; j < listLength; j++) {

            if (newCmtList[j].DepthLevel == 0) {
                continue;
            }

            for (var k = 0; k < listLength; k++) {

                if (newCmtList[k].ID == newCmtList[j].ParentID) {
                    newCmtList[k].cmt_children.push(newCmtList[j]);
                    break;
                }
            }
        }


        for (var i = (newCmtList.length - 1); i >= 0; i--) {
            if (newCmtList[i].DepthLevel != 0) {
                newCmtList.splice(i, 1);
                newCmtList[i].cmt_children.sort(dynamicSort("-ID"));
            }
        }


        //sort by newest on top
        newCmtList.sort(dynamicSort("-ID"));
        return newCmtList;
    }


    function showCommentReply() {
        if (rawCommentSet.length == 0 || !postRawData.CanComment) {
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



    Vue.component('comment-component', {
        props: ['comment'],
        template: '<div class="container">' +
            '            <div class="container-fluid">' +
            '                <div>' +
            '                    <span class="m-2 mr-0" style="margin-right:0 !important">Posted by</span>' +
            '                    <a style="margin-right:10px !important">[{{ comment.CreatedBy }}]</a>' +
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
                            ParentID: formData.ParentID,
                            CreatedBy: "me",
                            CreatedDate: dtStr,
                            Content: formData.Content,
                            IsEnabled: true
                        };

                        rawCommentSet.push(newCmt);
                        var cmtArrangedList = arrangeCommentTree(rawCommentSet);
                        vueInstance.comments = cmtArrangedList;

                        showCommentReply();
                    });

            }
        }
    });





    new Vue({
        el: "#post-container",
        data: {
            postID: postID,
            parentID: "",
            title: "",
            content: "",
            postTime: "",
            modifiedDate: "",
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
                    ParentID: parseInt(postID, 10)
                };

                var jsonData = JSON.stringify(formData);


                execCreateComment(jsonData)
                    .done(function (data) {
                        $("#post-reply textarea").val("");
                        $("#post-reply").toggle();

                        var dtStr = getTodayDateStr();


                        var newCmt = {
                            ID: data,
                            ParentID: formData.ParentID,
                            CreatedBy: "me",
                            CreatedDate: dtStr,
                            Content: formData.Content,
                            IsEnabled: true
                        };


                        rawCommentSet.push(newCmt);
                        var cmtArrangedList = arrangeCommentTree(rawCommentSet);
                        vueInstance.comments = cmtArrangedList;

                        showCommentReply();
                    })
            }
        },
        mounted: function () {
            vueInstance = this;
            var self = this;


            $.ajax({
                method: "POST",
                url: "/uhubapi/posts/GetByID?PostID=" + encodeURIComponent(postID)
            })
                .done(function (pstData) {
                    postRawData = pstData;
                    var clubID = pstData.ParentID;

                    self.parentID = pstData.ParentID;
                    self.title = htmlEncode(pstData.Name);
                    self.content = mdConverter.makeHtml(pstData.Content);
                    self.postTime = pstData.CreatedDate;
                    self.modifiedDate = pstData.ModifiedDate;


                    $("#post-container").style('display', null);
                    if (pstData.CanComment) {
                        $("#btn_ToggleReply").style('display', null);
                    }


                    //fetch comments if necessary
                    if (self.title != undefined && self.title != null && self.title != "") {
                        $.ajax({
                            method: "POST",
                            url: "/uhubapi/comments/GetByPost?PostID=" + encodeURIComponent(postID)
                        })
                            .done(function (cmtData) {
                                rawCommentSet = cmtData;
                                var cmtArrangedList = arrangeCommentTree(rawCommentSet);
                                //console.log(JSON.parse(JSON.stringify(cmtArrangedList)));

                                self.comments = cmtArrangedList;

                                showCommentReply();


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
