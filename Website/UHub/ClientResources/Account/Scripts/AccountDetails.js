﻿(function(){
	var userID = window.location.href.split('/').slice(-1)[0];
    var getUser = new Vue({
        el: "#user-info",
        data: function () {
            return {
            	user: {}
			}
		},
        mounted: function() {
            var self = this;
			$.ajax({
				method: "POST",
				url: "/uhubapi/users/GetByID?UserID=" + userID,
				success: function (data) {
					console.log(data);

					$(document).ready(function () {
						if (data.JobTitle == "" && data.Company == "") {
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