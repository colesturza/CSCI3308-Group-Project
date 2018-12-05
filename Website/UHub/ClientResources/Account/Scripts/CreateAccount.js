
(function () {


    var oldEmail = "";
    var oldUserObj = null;
    var oldResponseErr = null;

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
                $("#btn_CreateUser").attr("disabled", "disabled");
                $("html").css({ cursor: "wait" });
                var recapVal = grecaptcha.getResponse();

                var userObj = createUser.sendObj;

                if (userObj == userObjOld) {
                    alert(oldResponseErr);
                    return;
                }
                userObjOld = userObj;


                if (!$("#email").val().match(/^[\w-]+@([\w-]+\.)+[\w-]+$/)) {
                    oldResponseErr = 'Email Invalid';
                    alert(oldResponseErr);
                    $("#btn_CreateUser").removeAttr("disabled");
                    $("html").css({ cursor: "default" });
                    return;
                }
                else if (!$("#usr").val().match(/^\S{3,50}$/)) {
                    oldResponseErr = 'Username Invalid';
                    alert(oldResponseErr);
                    $("#btn_CreateUser").removeAttr("disabled");
                    $("html").css({ cursor: "default" });
                    return;
                }
                else if (!$("#pwd").val().match(/^{8,150}$/)) {
                    oldResponseErr = 'Password Invalid';
                    alert(oldResponseErr);
                    $("#btn_CreateUser").removeAttr("disabled");
                    $("html").css({ cursor: "default" });
                    return;
                }
                else if (!this.Name.val().match(/^(([ \u00c0-\u01ffA-z'\-])+){2,200}$/)) {
                    oldResponseErr = 'Name Invalid';
                    alert(oldResponseErr);
                    $("#btn_CreateUser").removeAttr("disabled");
                    $("html").css({ cursor: "default" });
                    return;
                }
                else if ($("#phone").val() != "" && !$("#phone").val().match(/^([0-1][ .-])?((\([0-9]{3}\)[ .-]?)|([0-9]{3}[ .-]?))([0-9]{3}[ .-]?)([0-9]{4})$/)) {
                    oldResponseErr = 'Phone Invalid';
                    alert(oldResponseErr);
                    $("#btn_CreateUser").removeAttr("disabled");
                    $("html").css({ cursor: "default" });
                    return;
                }
                else if (!$("#company").val().match(/^.{0,100}$/)) {
                    oldResponseErr = 'Company Invalid';
                    alert(oldResponseErr);
                    $("#btn_CreateUser").removeAttr("disabled");
                    $("html").css({ cursor: "default" });
                    return;
                }
                else if (!$("#job-title").val().match(/^.{0,100}$/)) {
                    oldResponseErr = 'Job Title Invalid';
                    alert(oldResponseErr);
                    $("#btn_CreateUser").removeAttr("disabled");
                    $("html").css({ cursor: "default" });
                    return;
                }



                $.ajax({
                    method: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    url: "/uhubapi/account/createuser",
                    headers: {
                        "g-recaptcha-response": recapVal
                    },
                    data: userObj,
                    complete: function () {
                        $("#btn_CreateUser").removeAttr("disabled");
                        $("html").css({ cursor: "default" });
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
                        oldResponseErr = data.responseJSON.status;
                        alert(oldResponseErr);
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
                majornames.sort();

                $("#autocomplete").autocomplete({
                    source: majornames
                });
            }
        });
    });


    $(function () {
        $("#grad-date").datepicker({
            changeMonth: true,
            changeYear: true
        });
        $("#grad-date").datepicker("option", "dateFormat", "yy-mm-dd");
    });



    registerInputValidator($("#email"), /^[\w-]+@([\w-]+\.)+[\w-]+$/);
    registerInputValidator($("#usr"), /^\S{3,50}$/);
    registerInputValidator($("#pwd"), /^{8,150}$/);
    registerInputValidator($("#firstname"), /^(([ \u00c0-\u01ffA-z'\-])+){1,100}$/);
    registerInputValidator($("#lastname"), /^(([ \u00c0-\u01ffA-z'\-])+){1,100}$/);
    registerInputValidator($("#phone"), /^([0-1][ .-])?((\([0-9]{3}\)[ .-]?)|([0-9]{3}[ .-]?))([0-9]{3}[ .-]?)([0-9]{4})$/);
    registerInputValidator($("#company"), /^.{0,100}$/);
    registerInputValidator($("#job-title"), /^.{0,100}$/);


})();