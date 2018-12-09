(function () {
	var getUser = new Vue({
		el: "#user-info",
		data: function () {
			return {
				user: {}
			}
		},
		mounted: function () {
			var self = this;
			$.ajax({
				method: "POST",
				url: "/uhubapi/users/GetMe",
				success: function (data) {
					console.log(data);


					$.ajax({
						method: "GET",
						url: "/uhubapi/schools/GetByID?SchoolID=" + data.SchoolID,
						success: function (data) {
							console.log(data)
							self.schoolID = data
						}
					})


					$(document).ready(function () {
						if (data.JobTitle == "" || data.JobTitle == null || data.Company == "" || data.Company == null) {
							$("#toggle_hide").hide();
						}
					});

					self.user = data;
				},
				statusCode: {
					404: function () {
						alert("User Not Found");
					}
				}
			})
		}
	});
})();