(function () {

    var examplePosts = [{ postid: 1, url: '#', subject: "LoremIpsum", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum." },
    { postid: 2, url: '#', subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum." },
    { postid: 3, url: '#', subject: "LoremIpsum", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum." },
    { postid: 4, url: '#', subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum." },
    { postid: 4, url: '#', subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum." },
    { postid: 4, url: '#', subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum." },
    { postid: 4, url: '#', subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum." }];
    var compsciPosts = [{ postid: 1, url: '#', subject: "LoremIpsum", postContent: "Foobar" },
    { postid: 2, url: '#', subject: "IpsumLorem", postContent: "Else{ Heart.Break() }" },
    { postid: 3, url: '#', subject: "LoremIpsum", postContent: "this.post = 'a post'" },
    { postid: 4, url: '#', subject: "IpsumLorem", postContent: "this.post = 'another post'" },
    { postid: 4, url: '#', subject: "IpsumLorem", postContent: "Exam avg: 70%" },
    { postid: 4, url: '#', subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum." },
    { postid: 4, url: '#', subject: "IpsumLorem", postContent: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eros mauris, mollis at fermentum mattis, interdum et ipsum." }];


    var exampleCommunities = [{ name: "Anthropology", url: "www.google.com" }, { name: "Computer Science B.S.", url: "#" }, { name: "Mechanical Engineering", url: "#" }];

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
        props: ['post'],
        template: `
   <div class="container w-70 mb-3">
       <a v-bind:href="'/Post' + post.ID">
        <h4> {{ post.Name }} </h4>
        <div v-html="post.Content"></div>
       </a>
   </div>`
    });

    var communityDropdown = new Vue({
        el: "#communityDrop",
        data: {
            communities: [],
            current: "Dropdown link"
        },
        methods: {
            getCommunities() {
                this.communities = exampleCommunities;
                console.log(this.communities);
            },
            /*updateDisplay is an incomplete function intended to change the name of the dropdown when a community is selected.
              Since we decided to update the dropdown based on the get request, there shouldn't be a need to use this
              The function is provided in case it is useful*/
            updateDisplay(newName) {
                document.getElementById('navbarDropdownMenuLink').innerHTML = newName
            }
        },
        beforeMount() {
            this.getCommunities()
        }
    });

var communityblock = new Vue({
    el: "#community-block",
    data: {
        commmunity: null,
    },
    methods: {
        getCommunity() {
            var communityRequest = $.ajax({
                method: "POST",
                url: "/uhubapi/schoolclubs/GetByID?ClubID=" + encodeURIComponent(window.location.href.split('/').slice(-1)[0]),
                statusCode: {
                    200: function (data) {
                        console.log(data);
                        this.community = data;
                    },
                    503: function () {
                        console.log("Internal Server Error");
                    }
                },
                error: function (error) {
                    console.log(error);
                }
            })
        }
    },
    beforeMount() {
        this.getCommunity();
    }
});

    new Vue({
        el: "#post-list",
        data: {
            posts: []
        },
        mounted: function () {
            var self = this;
            var mdConverter = new showdown.Converter();
            var id = encodeURIComponent(window.location.href.split('/').slice(-1)[0]);


            $.ajax({
                method: "POST",
                url: "/uhubapi/posts/GetAllByClub?ClubID=" + id,
                //dataType: "json",                 //No need to set dataType for this request because it accepts a queryString
                success: function (formData) {
                    console.log(formData);

                    for (var i = 0; i < formData.length; i++) {
                        formData[i].Content = mdConverter.makeHtml(formData[i].Content);
                    }
                    formData.sort(dynamicSort("-CreatedDate"));
                    self.posts = formData;

                },
                error: function (error) {
                    console.log(error);
                }
            })

        }
    });

})();