var communityRequest;
$.ajax({
    method: "POST",
    url: "/uhubapi/schoolclubs/GetAllBySchool",
    success: function (data) {
        console.log(data);
        communityRequest = data;
        console.log(communityRequest);
    },
    statusCode: {
        401: function () {
            communityRequest = null;
        }
    }
});

var homePosts;
$.ajax({
    method: "POST",
    url: "/uhubapi/posts/GetAllBySchool",
    success: function (data) {
        console.log(data);
        homePosts = data;
        console.log(homePosts);
    },
    statusCode: {
        401: function () {
            homePosts = null;
        }
    }
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