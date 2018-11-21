var communityDropdown = new Vue({
    el: "#communityDrop",
    data: {
        communities: []
    },
    mounted: function () {
        var self = this;
        $.ajax({
            method: "POST",
            url: "/uhubapi/schoolclubs/GetAllBySchool",
            success: function (data) {
                self.communities = data;
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
});

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