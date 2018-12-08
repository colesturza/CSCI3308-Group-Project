(function () {
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

                    if (data.length > 0) {

                        for (var i = 0; i < data.length; i++) {
                            data[i].Content = mdConverter.makeHtml(data[i].Content);
                        }
                        data.sort(dynamicSort("-CreatedDate"));


                        //set parentName
                        var clubSet = $("#navbarDropdownMenu a.dropdown-item");
                        for (var i = 0; i < data.length; i++) {
                            for (var j = 0; j < clubSet.length; j++) {
                                if (data[i].ParentID == $(clubSet[j]).attr("data-clubID")) {
                                    data[i].ClubName = clubSet[j].text();
                                }
                            }
                        }


                        self.posts = data;
                    }
                    else {
                        self.posts = [{
                            Name: "Nothing To See Here",
                            Content: "This school currently does not have any posts"
                        }];
                    }




                })
                //AJAX -> /uhubapi/posts/GetAllBySchool
                .fail(function (error) {
                    console.log(error);

                    self.posts = [{
                        Name: "Nothing To See Here",
                        Content: "Unfortunately, an error occured while fetching posts"
                    }];
                })
                //AJAX -> /uhubapi/posts/GetAllBySchool
                .always(function () {
                    $("#post-list").style('display', null);

                });
        }
    });

})();