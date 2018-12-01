new Vue({
    el: "#club-list",
    data: {
        clubs: []
    },
    mounted: function () {
        var self = this;

        $.ajax({
            method: "POST",
            url: "/uhubapi/SchoolClubs/GetAllBySchool",
            data: {},
            statusCode: {
                200: function (data) {
                    self.clubs = data;
                    self.clubs.sort(dynamicSort("Name"));
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
});
