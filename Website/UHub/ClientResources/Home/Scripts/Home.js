var communityRequest = $.ajax({
    method: "POST",
    url: "https://" + window.location.hostname + "dev.u-hub.life/uhubapi/schoolclubs/GetAllBySchool",
    headers: {auth: ""}
});

var homePosts = $.ajax({
    method: "POST",
    url: "https://" + window.location.hostname +  "dev.u-hub.life/uhubapi/posts/GetAllBySchool",
    headers: {auth: ""}
});

var examplePosts = [{ postid: 1, url:'#',subject: "LoremIpsum", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum." },
    {postid: 2, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 3, url:'#',subject: "LoremIpsum", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."}];

var exampleCommunities = [{name: "Anthropology",url: "www.google.com"},{name: "Computer Science B.S.", url: "#"}, {name: "Mechanical Engineering", url: "#"}];

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