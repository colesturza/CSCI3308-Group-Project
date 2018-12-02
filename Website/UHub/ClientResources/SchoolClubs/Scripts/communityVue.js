(function () {
    Vue.component('postlistcomp', {
        props: ['post'],
        template: `
   <div class="container bg-white mb-3">
       <a v-bind:href="'/Post/' + post.ID">
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
            community: null
        },
        methods: {
            getCommunity() {
                var self = this;
                var communityRequest = $.ajax({
                    method: "POST",
                    url: "/uhubapi/schoolclubs/GetByID?ClubID=" + encodeURIComponent(window.location.href.split('/').slice(-1)[0]),
                    statusCode: {
                        200: function (data) {
                            console.log(data);
                            self.community = data;
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