var communityRequest;
$.ajax({
    method: "POST",
    url: "/uhubapi/schoolclubs/GetAllBySchool",
    success: function (data) {
        communityRequest = data;
    },
    statusCode: {
        401: function () {
            alert("Unauthorized");
            communityRequest = null;
        }
    }
});

var homePosts = $.ajax({
    method: "POST",
    url: "/uhubapi/posts/GetAllBySchool"
});

var communityDropdown = new Vue({
    el: "#communityDrop",
    data: {
        communities: []
    },
    methods: {
        getCommunities(){
            this.communities = communityRequest;
            console.log(this.communities);
        }
    },
    beforeMount(){
        this.getCommunities()
    }
});

var postList = new Vue({
    el: "#post-list",
    data: {
        posts: []
    },
    methods: {
        getPosts(){
            this.posts = homePosts;
            console.log(this.posts);
        }
    },
    beforeMount(){
        this.getPosts()
    }
});