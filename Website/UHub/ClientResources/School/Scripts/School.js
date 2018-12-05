﻿(function () {
    var mdConverter = new showdown.Converter();
    setShowdownDefaults(mdConverter);


    new Vue({
        el: "#post-list",
        data: {
            posts: []
        },
        mounted: function () {
            var self = this;
            $.ajax({
                method: "POST",
                url: "/uhubapi/posts/GetAllBySchool"
            })
                //AJAX -> /uhubapi/posts/GetAllBySchool
                .done(function (data) {

                    for (var i = 0; i < data.length; i++) {
                        data[i].Content = mdConverter.makeHtml(data[i].Content);
                    }
                    data.sort(dynamicSort("-CreatedDate"));


                    self.posts = data;

                    $("#post-list").style('display', null);
                })
                //AJAX -> /uhubapi/posts/GetAllBySchool
                .fail(function (error) {
                    console.log(error);
                });
        }
    });

})();