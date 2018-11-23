var oldEmail = "";

var createUser = new Vue({
    el: "#userForm",
    data: {
        Username: "",
        Password: "",
        Email: "",
        Name: "",
        Major: "",
        PhoneNumber: "",
        Year: "",
        GradDate: "",
        Company: "",
        JobTitle: ""
    },
    methods: {
        readRefs() {
            let refs = this.$refs;
            this.Username = refs.inputUSR.value;
            this.Password = refs.inputPWD.value;
            this.Email = refs.inputEmail.value;
            this.Name = refs.inputFirst.value + refs.inputLast.value;
            this.Major = refs.inputMajor.value;
            this.PhoneNumber = refs.inputPhone.value;
            this.Year = refs.inputYear.value;
            this.GradDate = refs.inputGrad.value;
            this.Company = refs.inputCompany.value;
            this.JobTitle = refs.inputJob.value;
            this.sendObj = JSON.stringify(this.$data);
        },
        sendData() {
            $("#btn_CreateUser").attr("disabled", "disabled");


            $.ajax({
                method: "POST",
                url: "/uhubapi/account/createuser",
                data: createUser.sendObj,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                complete: function () {
                    $("#btn_CreateUser").removeAttr("disabled");
                },
                success: function (data) {
                    
                    alert(data.status);

                    if (data.canLogin === true) {
                        window.location.href = "/";
                    }
                    else {
                        window.location.href = "/Account/Confirm/New";
                    }
                },
                error: function (data) {
                    alert(data.responseJSON.status);
                }
            });
        }
    }
});
/*
    When email is unfocused, sends ajax request to get list of majors to supply the dropdown
 */
$("#email").blur(function () {
    var majornames = [];
    var val = $(this).val();
    if (val == undefined || val == null || val == "") {
        return;
    }
    if (val === oldEmail) {
        return;
    }

    oldEmail = val;

    $.ajax({
        method: "get",
        url: "/uhubapi/schoolmajors/GetAllByEmail",
        data: {
            email: val
        },
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                if (data[i].IsEnabled === true) {
                    majornames.push(data[i].Name);
                }
            }

            $("#autocomplete").autocomplete({
                source: majornames
            });
        }
    });
});

$(function () {
    $("#grad-date").datepicker();
    $("#grad-date").datepicker("option", "dateFormat", "yy-mm-dd");
});