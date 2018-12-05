(function () {

    var commID = encodeURIComponent(window.location.href.split('/').slice(-1)[0]);


    var mdConverter = new showdown.Converter();
    setShowdownDefaults(mdConverter);



    Vue.component('postlistcomp', {
        props: ['post'],
        template:
            `
           <div class="container bg-white mb-3" style="padding: .75rem 1.25rem">
               <a v-bind:href="'/Post/' + post.ID">
                <h4> {{ post.Name }} </h4>
               </a>
               <div v-html="post.Content"></div>
           </div>
        `
    });

    var communityblock = new Vue({
        el: "#community-block",
        data: {
            community: null
        },
        methods: {
            getCommunity() {
                var self = this;


                $.ajax({
                    method: "POST",
                    url: "/uhubapi/schoolclubs/GetByID?ClubID=" + encodeURIComponent(commID),
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
                });
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


            $.ajax({
                method: "POST",
                url: "/uhubapi/posts/GetAllByClub?ClubID=" + encodeURIComponent(commID)
            })
                //AJAX -> /uhubapi/posts/GetAllByClub
                .done(function (formData) {

                    if (data.length > 0) {

                        for (var i = 0; i < formData.length; i++) {
                            formData[i].Content = mdConverter.makeHtml(formData[i].Content);
                        }
                        formData.sort(dynamicSort("-CreatedDate"));
                        self.posts = formData;
                    }
                    else {
                        self.posts = [{
                            Name: "Nothing To See Here",
                            Content: "This club currently does not have any posts"
                        }];
                    }
                    
                })
                //AJAX -> /uhubapi/posts/GetAllByClub
                .fail(function (error) {
                    console.log(error);

                    self.posts = [{
                        Name: "Nothing To See Here",
                        Content: "Unfortunately, an error occured while fetching posts"
                    }];
                })
                //AJAX -> /uhubapi/posts/GetAllByClub
                .always(function(){

                    $("#body-content").style('display', null);

                });
        }
    });
})();