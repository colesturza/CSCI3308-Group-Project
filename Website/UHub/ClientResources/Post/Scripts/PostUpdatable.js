(function () {

    var simplemde;
    var postRawData;
    var jsonPostDataOld = null;
    var oldResponseErr = null;


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


    function getPostUpdateData() {
        postRawData.Content = simplemde.value();

        return {
            ID: postRawData.ID,
            Name: postRawData.Name,
            Content: postRawData.Content,
            IsPublic: postRawData.IsPublic,
            CanComment: postRawData.CanComment
        };
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
            '                    <span>Parent:{{ comment.ParentID }}</span>' +
            '                    <span>Depth:{{ comment.DepthLevel }}</span>' +
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
            '               <div>' +
            '                   <comment-component v-for="comment in comment.cmt_children" :key="comment.id" v-bind:comment="comment"><comment-component>' +
            '               </div>' +
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

                        vueInstance.comments.splice(0, 0, newCmt);
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
                    //AJAX -> /uhubapi/comments/Create
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

                        vueInstance.comments.splice(0, 0, newCmt);
                    });

            },
            // getCommentById and getCommentDepth  are helper functions for arrangeCommentTree
            // Returns matching comment object
            getCommentById: function(cmtID, cmtList) {
                for (var j = 0; j < cmtList.length; j++){
                    if(cmtList[j].ID == cmtID) {
                        return cmtList[j];
                    }
                }
            },
            // Finds the comment's degrees of separation from post
            getCommentDepth: function (theCmt, cmtList) {
                console.log(theCmt)

                var depthLevel = 0;
                while(theCmt.ParentID != postID) {
                    theCmt = this.getCommentById(theCmt.ParentID, cmtList);
                    depthLevel++;
                }
                return depthLevel;
            },
            // Arranges array so that children are within parent comments
            arrangeCommentTree: function(cmtList) {
                var maxDepth=0;
                var listLength = cmtList.length;
                

                for(var i = listLength - 1; i >= 0; i--) {
                    if(!cmtList[i].IsEnabled) {
                        cmtList.splice(i, 1);
                    }
                }
                console.log(cmtList)

                for(var i = 0; i < listLength; i++) {
                    cmtList[i].cmt_children = [];
                    cmtList[i].DepthLevel = this.getCommentDepth(cmtList[i], cmtList);
                    if (cmtList[i] > maxDepth) {
                        maxDepth = cmtList.DepthLevel;
                    }
                }
                for(var i = maxDepth; i > 0; i--) {
                    for(var j=0; j < listLength; j++) {
                        if(cmtList[j].DepthLevel == i) {
                            for(var k=0; k < listLength; k++) {
                                if(cmtList[j].ParentID == cmtList[k].ID) {
                                    cmtList[k].cmt_children.push(cmtList[j]);
                                }
                            }
                        }
                    }
                }
                for(var i = (cmtList.length - 1); i >= 0; i--) {
                    if(cmtList[i].ParentID != postID) {
                        cmtList.slice(i, 1);
                    }
                }

                console.log(cmtList)

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
                ///AJAX -> uhubapi/posts/GetByID
                .done(function (pstData) {
                    postRawData = pstData;

                    var clubID = pstData.ParentID;

                    self.parentID = pstData.ParentID;
                    self.title = htmlEncode(pstData.Name);
                    window.setTimeout(function () {
                        simplemde.value(pstData.Content);
                        //set old value to prevent clean updates
                        oldResponseErr = 'Nothing to Update';
                        jsonPostDataOld = JSON.stringify(getPostUpdateData());
                    }, 1);
                    self.postTime = pstData.CreatedDate;


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
                            //AJAX -> /uhubapi/comments/GetByPost
                            .done(function (cmtData) {
                                console.log(cmtData)

                                //cmtData.sort(dynamicSort("-CreatedDate"));
                                self.comments = self.arrangeCommentTree(cmtData);

                                if (cmtData.length == 0 || !postRawData.CanComment) {
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
                            //AJAX -> /uhubapi/comments/GetByPost
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
                ///AJAX -> uhubapi/posts/GetByID
                .fail(function (jqAjax, errorText) {
                    alert("Error " + errorText);
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




    function setWaitState() {
        $("#btn_UpdatePost").attr("disabled", "disabled");
        $("html").css({ cursor: "default" });
    }

    function clearWaitState() {
        $("#btn_UpdatePost").removeAttr("disabled");
        $("html").css({ cursor: "default" });
    }


    function processInputValidation(formData) {
        if (!formData.Content.match(RgxPtrns.Post.CONTENT)) {
            oldResponseErr = 'Post Content Invalid';
            alert(oldResponseErr);
            return false;
        }

        return true;
    }


    function sendData(formData) {
        setWaitState();

        var jsonPostData = JSON.stringify(formData);

        if (jsonPostData == jsonPostDataOld) {
            alert(oldResponseErr);
            clearWaitState();
            return;
        }
        jsonPostDataOld = jsonPostData;


        if (!processInputValidation(formData)) {
            clearWaitState();
            return false;
        }
        


        $.ajax({
            method: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: jsonPostData,
            url: "/uhubapi/posts/Update"
        })
            //AJAX -> /uhubapi/posts/Update
            .done(function (data) {
                oldResponseErr = 'Nothing to Update';
                alert(data);
            })
            //AJAX -> /uhubapi/posts/Update
            .fail(function (data) {
                oldResponseErr = data.responseJSON;
                alert(oldResponseErr);
            })
            //AJAX -> /uhubapi/posts/Update
            .always(function (data) {
                clearWaitState();
            });
    }



    $("#btn_UpdatePost").click(function () {
        var data = getPostUpdateData();
        sendData(data);
    });




    registerInputValidator($("#txt_PostArea"), RgxPtrns.Post.CONTENT);



})();
