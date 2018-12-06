(function () {
    var mdConverter = new showdown.Converter();
    setShowdownDefaults(mdConverter);


    new Vue({
        el: "#post-versions",
        data: {
            postVersions: []
        },
        mounted: function () {
            var self = this;
            $.ajax({
                method: "POST",
                url: "/uhubapi/posts/GetRevisionByID?PostID=" + encodeURIComponent(postID)
            })
            //AJAX -> /uhubapi/posts/GetRevisionByID
            .done(function (data) {
                if (data.length > 0) {

                    for (var i = 0; i < data.length; i++) {
                        data[i].Content = mdConverter.makeHtml(data[i].Content);
                    }
                    data.sort(dynamicSort("-CreatedDate"));

                    self.postVersions = data;
                } else {
                    self.postVersions = [{
                        Name: "Nothing To See Here",
                        Content: "This post does not exist."
                    }];
                }
            })
            //AJAX -> /uhubapi/posts/GetRevisionByID
            .fail(function (error) {
                console.log(error);

                self.posts = [{
                    Name: "Nothing To See Here",
                    Content: "Unfortunately, an error occured while fetching this post's previous versions"
                }];
            })
            //AJAX -> /uhubapi/posts/GetRevisionByID
            .always(function () {
                $("#post-versions").style('display', null);
            });
        }
    });
})();