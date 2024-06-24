/*
    sujaytravelapps.com
    © 2019, Sujay V Sarma. All rights reserved.
    Product : sujaytravelapps.com
    About this file: This script defines the MapUXDialog type

    We are using JSDoc to perform type hinting for VS IDE
*/


/**
 * Types of dialogs we support
 * @enum {Number}
 */
var EnumDialogType = {
    Information: 0,
    Warning: 1,
    Error: 2,
    Question: 3
}

/**
 * The MapUXDialog type
 */
class MapUXDialog {

    /**
     * Constructor
     * @param {string} selector - Id or class selector of the container 
     */
    constructor(selector) {
        $(selector).html(
            '<div id="mapUxMessageBox" class="modal fade" tabindex="-1" role="dialog">' +
            '<div class="modal-dialog modal-dialog-centered modal-lg" role="document"><div class="modal-content"></div></div></div>'
        );

        $('#mapUxMessageBox').modal(
            {
                backdrop: 'static',
                keyboard: false,
                focus: true,
                show: false
            }
        );
    }

    /**
     * Show the dialog
     * @param {EnumDialogType} type - Type of dialog to show
     * @param {string} caption - The dialog title
     * @param {string} content - Content of the dialog, can be Html
     * @param {any} callback - Callback function, called only for type == Question
     * @param {any} callbackState - An object to pass in to the callback function
     */
    Show(type, caption, content, callback, callbackState) {
        var iconName = 'info-sign';
        var buttons = new Array();
        switch (type) {
            case EnumDialogType.Information:
                iconName = 'info-sign';
                break;

            case EnumDialogType.Warning:
                iconName = 'warning-sign';
                break;

            case EnumDialogType.Error:
                iconName = 'stop';
                break;

            case EnumDialogType.Question:
                iconName = 'question-sign';
                buttons = new [
                    {
                        classList: 'btn btn-primary',
                        id: 'mapUxMessageBox_yesButton',
                        caption: 'Yes'
                    }
                ];

                if (callback !== null) {
                    $('#mapUxMessageBox_yesButton').on('click', () => {
                        $('#mapUxMessageBox').modal('hide');
                        callback(
                            {
                                button: 'Yes',
                                state: callbackState
                            }
                        );
                    });
                }
                break;
        }

        this._createDialog(iconName, caption, content, buttons);
        $('#mapUxMessageBox').modal('show');
    }


    _createDialog(iconName, caption, content, buttons) {
        var dialogHtml =
            '<div class="modal-header">' +
            '<h4 class="modal-title"><span class="glyphicon glyphicon-' + iconName + '"></span>' + caption + '</h4>' +
            '<button type="button" class="close" data-dismiss="modal">&times;</button></div><div class="modal-body small">' + content + '</div>' +
            '<div class="modal-footer"><button type="button" class="btn btn-info border-dark text-white font-weight-bold" data-dismiss="modal">Close</button>';

        for (var i = 0; i < buttons.length; i++) {
            dialogHtml += '<button type="button" class="btn border-dark text-white font-weight-bold ' + buttons[i].classList + '" id="' + buttons[i].id + '">' +
                buttons[i].caption + '</button>';
        }

        dialogHtml += '</div>';

        $('#mapUxMessageBox div.modal-content').html(dialogHtml);
    }

}