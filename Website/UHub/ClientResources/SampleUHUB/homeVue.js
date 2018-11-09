var examplePosts = [{rating: 0, postid: 1, url:'#',subject: "LoremIpsum", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum." },
    {rating: 0, postid: 2, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {rating: 0, postid: 3, url:'#',subject: "LoremIpsum", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {rating: 0, postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {rating: 0, postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {rating: 0, postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {rating: 0, postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."}];

var exampleCommunities = [{name: "Anthropology",url: "www.google.com"},{name: "Computer Science B.S.", url: "#"}, {name: "Mechanical Engineering", url: "#"}];
/*

var postRequest = $.get("endpoint", function (data) {
    console.log(data);
});

var communityRequest = $.get("endpoint", function(data){
   console.log(data);
});

*/
$.ajax({
  url: "https://images.pexels.com/photos/46710/pexels-photo-46710.jpeg?cs=srgb&dl=beach-sand-summer-46710.jpg&fm=jpg"
  ,data
})
var communityDropdown = new Vue({
    el: "#communityDrop",
    data: {
        communities: []
    },
    methods: {
        getCommunities(){
            this.communities = exampleCommunities;
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
            this.posts = examplePosts;
            console.log(this.posts);
        }
    },
    beforeMount(){
        this.getPosts()
    },
    like(){
      this.posts.rating += 1;
    },
    dislike(){
      this.posts.rating -= 1;
    }
});
