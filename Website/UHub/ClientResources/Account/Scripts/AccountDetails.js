(function(){
	var userID = window.location.href.split('/').slice(-1)[0];
    var getUser = new Vue({
        el: "#user-info",
        data: {
            user: {}
        },
        methods: {
            user_get() {
                $.ajax({
                    method: "POST",
                    url: "/uhubapi/users/GetByID?UserID=" + userID,
                })
					.done(function(data) {
                    console.log(data);
                    let chosenUser = this.user;
                    chosenUser.Username = data.Username;
                    chosenUser.Major = data.Major;
                    chosenUser.Year = data.Year;
                    chosenUser.GradDate = data.GradeDate;
                    chosenUser.JobTitle = data.JobTitle;
                    chosenUser.Company = data.Company;
                });
            }
        },
        beforeMount() {
            this.user_get()
        }
    });
})();