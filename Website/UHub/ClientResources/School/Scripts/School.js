var postList = new Vue({
    el: "#post-list",
    data: {
        posts: []
    },
    mounted: function () {
        var self = this;
        $.ajax({
            method: "POST",
            url: "/uhubapi/posts/GetAllBySchool",
            success: function (data) {
                self.posts = data;
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
});