function registerInputValidator(obj, rgxStr, allowEmpty) {

    if (allowEmpty == undefined || allowEmpty == null) {
        allowEmpty = false;
    }

    var outline = $(obj).style('outline-color');
    var border = $(obj).style('border-color');
    var shadow = $(obj).style('box-shadow');
    $(obj).attr('data-baseStyles', outline + "|" + border + "|" + shadow);


    var validate = function () {

        var col = null;
        var rgb = null;
        var isValid = false;

        if (allowEmpty && $(this).val() == "") {
            var baseStyles = $(this).attr('data-baseStyles');
            var bStyleSet = baseStyles.split('|');

            $(this).style('outline-color', bStyleSet[0]);
            $(this).style('border-color', bStyleSet[1]);
            $(this).style('box-shadow', bStyleSet[2]);

            $(this).attr('data-isValid', 'true');

            return;
        }
        else {
            if ($(this).val().match(rgxStr)) {
                isValid = 'true';
                col = 'green';
                rgb = 'rgba(0, 255, 0, .2)';
            }
            else {
                isValid = 'false';
                col = 'red';
                rgb = 'rgba(255, 0, 0, .2)';
            }
        }

        $(this).attr('data-isValid', isValid);
        $(this).style('outline-color', col, "important");
        $(this).style('border-color', col, "important");
        $(this).style('box-shadow', "0 0 .1rem 0.2rem " + rgb, "important");
    }



    window.setInterval(validate, 100);
    $(obj).on('change, keydown, keyup', validate);
}