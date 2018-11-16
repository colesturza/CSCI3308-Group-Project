var communityDropdown = new Vue({
    el: "#communityDrop",
    data() {
        var communityRequest = $.ajax({
            method: "POST",
            url: "/uhubapi/schoolclubs/GetAllBySchool",
            statusCode: {
                401: function () {
                    communityRequest = [];
                }
            }
        });

        return {
            communities: communityRequest
        }
    }
});

var postList = new Vue({
    el: "#post-list",
    data() {
        var homePosts = $.ajax({
            method: "POST",
            url: "/uhubapi/posts/GetAllBySchool",
            statusCode: {
                401: function () {
                    homePosts = [];
                }
            }
        });
        return {
            posts: homePosts
        }
    }
});