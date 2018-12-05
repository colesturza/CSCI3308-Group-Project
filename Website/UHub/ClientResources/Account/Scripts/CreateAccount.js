
(function () {


    var RGX_EMAIL = /^[\w-]+@([\w-]+\.)+[\w-]+$/;
    var RGX_USERNAME = /^\S{3,50}$/;
    var RGX_PSWD = /^{8,150}$/;
    var RGX_NAME = /^(([ \u00c0-\u01ffA-z'\-])+){2,200}$/;
    var RGX_PHONE = /^([0-1][ .-])?((\([0-9]{3}\)[ .-]?)|([0-9]{3}[ .-]?))([0-9]{3}[ .-]?)([0-9]{4})$/;
    var RGX_COMPANY = /^.{0,100}$/;
    var RGX_JOB_TITLE = /^.{0,100}$/;


    var oldEmail = "";
    var oldUserObj = null;
    var oldResponseErr = null;



    function setWaitState() {
        $("#btn_CreateUser").removeAttr("disabled");
        $("html").css({ cursor: "default" });
    }

    function clearWaitState() {
        $("#btn_CreateUser").removeAttr("disabled");
        $("html").css({ cursor: "default" });
    }


    function processInputValidation(formData) {


        if (!formData.Email.match(RGX_EMAIL)) {
            oldResponseErr = 'Email Invalid';
            alert(oldResponseErr);
            clearWaitState();
            return;
        }
        else if (!formData.Username.match(RGX_USERNAME)) {
            oldResponseErr = 'Username Invalid';
            alert(oldResponseErr);
            clearWaitState();
            return;
        }
        else if (!formData.Password.match(RGX_PSWD)) {
            oldResponseErr = 'Password Invalid';
            alert(oldResponseErr);
            clearWaitState();
            return;
        }
        else if (!formData.Name.match(RGX_NAME)) {
            oldResponseErr = 'Name Invalid';
            alert(oldResponseErr);
            clearWaitState();
            return;
        }
        else if (formData.PhoneNumber != "" && !formData.PhoneNumber.match(RGX_PHONE)) {
            oldResponseErr = 'Phone Invalid';
            alert(oldResponseErr);
            clearWaitState();
            return;
        }
        else if (!formData.Company.match(RGX_COMPANY)) {
            oldResponseErr = 'Company Invalid';
            alert(oldResponseErr);
            clearWaitState();
            return;
        }
        else if (!formData.JobTitle.match(RGX_JOB_TITLE)) {
            oldResponseErr = 'Job Title Invalid';
            alert(oldResponseErr);
            clearWaitState();
            return;
        }
    }





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
            readRefs: function () {
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
            sendData: function () {
                setWaitState();
                var recapVal = grecaptcha.getResponse();

                var userObj = createUser.sendObj;

                if (userObj == userObjOld) {
                    alert(oldResponseErr);
                    clearWaitState();
                    return;
                }
                userObjOld = userObj;


                if (!processInputValidation(this)) {
                    return;
                }



                $.ajax({
                    method: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    url: "/uhubapi/Account/CreateUser",
                    headers: {
                        "g-recaptcha-response": recapVal
                    },
                    data: userObj
                })
                    //AJAX -> "/uhubapi/Account/CreateUser"
                    .done(function (data) {

                        alert(data.status);

                        if (data.canLogin === true) {
                            window.location.href = "/";
                        }
                        else {
                            window.location.href = "/Account/Confirm/New";
                        }
                    })
                    //AJAX -> "/uhubapi/Account/CreateUser"
                    .fail(function (data) {
                        oldResponseErr = data.responseJSON.status;
                        alert(oldResponseErr);
                    })
                    //AJAX -> "/uhubapi/Account/CreateUser"
                    .always(function () {
                        $("#btn_CreateUser").removeAttr("disabled");
                        $("html").css({ cursor: "default" });
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
            }
        })
            //AJAX -> /uhubapi/schoolmajors/GetAllByEmail
            .done(function (data) {
                for (var i = 0; i < data.length; i++) {
                    if (data[i].IsEnabled === true) {
                        majornames.push(data[i].Name);
                    }
                }
                majornames.sort();

                $("#autocomplete").autocomplete({
                    source: majornames
                });
            });
    });



    $(function () {
        $("#grad-date").datepicker({
            changeMonth: true,
            changeYear: true
        });
        $("#grad-date").datepicker("option", "dateFormat", "yy-mm-dd");
    });



    registerInputValidator($("#email"), RGX_EMAIL);
    registerInputValidator($("#usr"), RGX_USERNAME);
    registerInputValidator($("#pwd"), RGX_PSWD);
    registerInputValidator($("#firstname"), RGX_NAME);
    registerInputValidator($("#lastname"), RGX_NAME);
    registerInputValidator($("#phone"), RGX_PHONE);
    registerInputValidator($("#company"), RGX_COMPANY);
    registerInputValidator($("#job-title"), RGX_JOB_TITLE);


})();