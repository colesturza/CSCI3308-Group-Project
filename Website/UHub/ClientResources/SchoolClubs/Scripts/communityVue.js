var examplePosts = [{ postid: 1, url:'#',subject: "LoremIpsum", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum." },
    {postid: 2, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 3, url:'#',subject: "LoremIpsum", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."}];
var compsciPosts = [{ postid: 1, url:'#',subject: "LoremIpsum", postContent: "Foobar" },
    {postid: 2, url:'#',subject: "IpsumLorem", postContent: "Else{ Heart.Break() }"},
    {postid: 3, url:'#',subject: "LoremIpsum", postContent: "this.post = 'a post'"},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "this.post = 'another post'"},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Exam avg: 70%"},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."},
    {postid: 4, url:'#',subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum."}];


var exampleCommunities = [{name: "Anthropology",url: "www.google.com"},{name: "Computer Science B.S.", url: "#"}, {name: "Mechanical Engineering", url: "#"}];

/*Sample variable to hold all of the values of the get request*/
var clubData = {
  id: Number,
  schoolID: Number,
  isEnabled: Boolean,
  isReadOnly: Boolean,
  name: String,
  description: String,
  createdDate: Number,
  modifiedDate: Number
}

/*Prototype for a post list component.*/
Vue.component('postlistcomp', {
  template: `
   <ul  class="list-group list-group-flush w-70 float-md-left">
   <li class="list-group-item mb-3"  v-for="post in posts">
       <h4> {{ post.subject }} </h4>
       <p> {{ post.postContent }} </p>
   </li>
   </ul>`,
  data: function() {
    return {
      posts: examplePosts
    }
  }
});

var communityDropdown= new Vue({
    el: "#communityDrop",
    data: {
        communities: [],
        current: "Dropdown link"
    },
    methods: {
        getCommunities(){
            this.communities = exampleCommunities;
            console.log(this.communities);
        },
        /*updateDisplay is an incomplete function intended to change the name of the dropdown when a community is selected.
          Since we decided to update the dropdown based on the get request, there shouldn't be a need to use this
          The function is provided in case it is useful*/
        updateDisplay(newName){
          document.getElementById('navbarDropdownMenuLink').innerHTML = newName
        }
    },
    beforeMount(){
        this.getCommunities()
    }
});

var descript = "This is a mounted description"

var communityblock = new Vue({
  el: "#community-block",
  data: {
    description: "Default Community Description"
  },
  methods: {
    getDescription(){
      this.description = descript;
    }
  },
  /*Updates the description when the instance is mounted to the value in descript.
    This should be set up so we can update the community description on the get request*/
  beforeMount()
  {
    description = this.getDescription();
  }
});

var postList = new Vue({
    el: "#post-list",
    data: {
        posts: []
    },
    methods: {
        getPosts(){
            var postRequest = $.ajax({
                method: "POST",
                url: "uhubapi/posts/GetAllByClub",
                data: url.split('/').slice(-1)[0],
                dataType: "json",
                statusCode: {
                    200: function () {
                        this.posts = postRequest.response;
                    },
                    503: function() {
                        console.log("Internal Server Error");
                    }
                },
                error: function(error) {
                    console.log(error);
                }
            })
        }
    },
    beforeMount()
    {
        this.getPosts()
    }
});
