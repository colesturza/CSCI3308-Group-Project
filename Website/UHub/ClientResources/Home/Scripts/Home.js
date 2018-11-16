var communityDropdown = new Vue({
    el: "#communityDrop",
    data() {
        var communityRequest = $.ajax({
            method: "POST",
            url: "/uhubapi/schoolclubs/GetAllBySchool"
        });

        console.log(communityRequest);

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
            url: "/uhubapi/posts/GetAllBySchool"
        });

        console.log(homePosts);

        return {
            posts: homePosts
        }
    }
});