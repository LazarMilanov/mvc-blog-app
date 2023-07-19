$(document).ready(function () {
    $.ajax({
        url: 'https://api.github.com/emojis',
        async: false
    }).then(function (data) {
        window.emojis = Object.keys(data);
        window.emojiUrls = data;
        });

    /*var v = $('#postForm').validate({
        // exclude it from validation
        ignore: ":hidden:not(.textarea-editor),.note-editable.panel-body"
    });*/

    var myElement = $('.textarea-editor');

    // Initialize Editor
    myElement.summernote({
        height: 300,
        minHeight: null,
        maxHeight: null,
        popover: {
            image: [],
            link: [],
            air: []
        },
        /*callbacks: {
            onChange: function (contents, $editable) {
                // Note that at this point, the value of the `textarea` is not the same as the one
                // you entered into the summernote editor, so you have to set it yourself to make
                // the validation consistent and in sync with the value.
                myElement.val(myElement.summernote('isEmpty') ? "" : contents);

                // You should re-validate your element after change, because the plugin will have
                // no way to know that the value of your `textarea` has been changed if the change
                // was done programmatically.
                v.element(myElement);
            }
        },*/
        placeholder: 'Create your post',
        focus: true  
        /*hint: {
            match: /:([\-+\w]+)$/,
            search: function (keyword, callback) {
                callback($.grep(emojis, function (item) {
                    return item.indexOf(keyword) === 0;
                }));
            },
            template: function (item) {
                var content = emojiUrls[item];
                return '<img src="' + content + '" width="20" /> :' + item + ':';
            },
            content: function (item) {
                var url = emojiUrls[item];
                if (url) {
                    return $('<img />').attr('src', url).css('width', 20)[0];
                }
                return '';
            }
        }*/
    });
}); 