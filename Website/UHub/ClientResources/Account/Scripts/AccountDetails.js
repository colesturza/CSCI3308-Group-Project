
var getUser = new Vue({
	el: "#user-info",
	data: {
		user: {}
	},
	mounted: function () {
		var self = this;
		$.ajax({
			method: "POST",
			url: "/uhubapi/users/GetByID?UserID=" + encodeURIComponent(window.location.href.split('/').slice(-1)[0]),
			success: function (data) {
				this.user = data
			}
		});
	}
});