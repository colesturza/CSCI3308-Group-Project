/*
    The ajax to grab all majors/communities
 */
var groupsRequest = $.ajax({
    method: "",
    url: "#",
}).done(function(){
});

var communityDropdown = new Vue({
    el: "#communityDrop",
    data: {
        communities: []
    },
    methods: {
        getCommunities(){
            this.communities = groupsRequest;
        }
    },
    beforeMount(){
        this.getCommunities()
    }
});