// jQuery plugin to prevent double submission of forms
jQuery.fn.preventDoubleSubmission = function () {
    $(this).on('submit', function (e) {
        var $form = $(this);

        if ($form.data('submitted') === true) {
            // Previously submitted - don't submit again
            e.preventDefault();
        } else {
            // Mark it so that the next submit can be ignored
            $form.data('submitted', true);
        }
    });
    return this;
};


(function ($) {
    if ($.fn.style) {
        return;
    }

    // Escape regex chars with \
    var escape = function (text) {
        return text.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&");
    };

    // For those who need them (< IE 9), add support for CSS functions
    var isStyleFuncSupported = !!CSSStyleDeclaration.prototype.getPropertyValue;
    if (!isStyleFuncSupported) {
        CSSStyleDeclaration.prototype.getPropertyValue = function (a) {
            return this.getAttribute(a);
        };
        CSSStyleDeclaration.prototype.setProperty = function (styleName, value, priority) {
            this.setAttribute(styleName, value);
            var priority = typeof priority != 'undefined' ? priority : '';
            if (priority != '') {
                // Add priority manually
                var rule = new RegExp(escape(styleName) + '\\s*:\\s*' + escape(value) +
                    '(\\s*;)?', 'gmi');
                this.cssText =
                    this.cssText.replace(rule, styleName + ': ' + value + ' !' + priority + ';');
            }
        };
        CSSStyleDeclaration.prototype.removeProperty = function (a) {
            return this.removeAttribute(a);
        };
        CSSStyleDeclaration.prototype.getPropertyPriority = function (styleName) {
            var rule = new RegExp(escape(styleName) + '\\s*:\\s*[^\\s]*\\s*!important(\\s*;)?',
                'gmi');
            return rule.test(this.cssText) ? 'important' : '';
        }
    }

    // The style function
    $.fn.style = function (styleName, value, priority) {
        // DOM node
        var node = this.get(0);
        // Ensure we have a DOM node
        if (typeof node == 'undefined') {
            return this;
        }
        // CSSStyleDeclaration
        var style = this.get(0).style;
        // Getter/Setter
        if (typeof styleName != 'undefined') {
            if (typeof value != 'undefined') {
                // Set style property
                priority = typeof priority != 'undefined' ? priority : '';
                style.setProperty(styleName, value, priority);
                return this;
            } else {
                // Get style property
                return style.getPropertyValue(styleName);
            }
        } else {
            // Get CSSStyleDeclaration
            return style;
        }
    };
})(jQuery);



function dynamicSort(property) {
    var sortOrder = 1;
    if (property[0] === "-") {
        sortOrder = -1;
        property = property.substr(1);
    }
    return function (a, b) {
        var result = (a[property] < b[property]) ? -1 : (a[property] > b[property]) ? 1 : 0;
        return result * sortOrder;
    };
}


function setShowdownDefaults(converter) {
    converter.setOption('literalMidWordUnderscores', 'true');
    converter.setOption('tables', 'true');
    converter.setOption('tasklists', 'true');
    converter.setOption('backslashEscapesHTMLTags', 'true');
    converter.setOption('literalMidWordUnderscores', 'true');
}


function registerInputValidator(obj, rgxStr, allowEmpty) {

    if (allowEmpty == undefined || allowEmpty == null) {
        allowEmpty = false;
    }

    $(obj).on('change, keydown, keyup', function () {

        var col = null;
        var rgb = null;


        if (allowEmpty && $(this).val() == "") {
            $(this).style('outline-color', null);
            $(this).style('border-color', null);
            $(this).style('box-shadow', null);

            return;
        }
        else {
            if ($(this).val().match(rgxStr)) {
                col = 'green';
                rgb = 'rgba(0, 255, 0, .2)';
            }
            else {
                col = 'red';
                rgb = 'rgba(255, 0, 0, .2)';
            }
        }


        $(this).style('outline-color', col, "important");
        $(this).style('border-color', col, "important");
        $(this).style('box-shadow', "0 0 .1rem 0.2rem " + rgb, "important");
    });
}


function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}