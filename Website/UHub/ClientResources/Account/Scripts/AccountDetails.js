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
                    this.user = data;
                });
            }
        },
        beforeMount() {
            this.user_get()
        }
    });
})();