(function () {
    'use strict';
    window.swal_driver = {
        Loading: (header, message) => {
            Swal.fire({
                title: header,
                html: message,
                allowOutsideClick: false,
                didOpen: () => { Swal.showLoading(); },
            });
        },
        Alert: async (header, message) => {
            return new Promise(function (resolve, reject) {
                Swal.fire({
                    title: header,
                    text: message,
                    icon: 'success',
                    didOpen: () => { Swal.hideLoading(); },
                }).then(function () {
                    resolve(true);
                });
            });
        },
        Error: async (header, message) => {
            return new Promise(function (resolve, reject) {
                Swal.fire({
                    title: header,
                    text: message,
                    icon: 'error',
                    didOpen: () => { Swal.hideLoading(); },
                }).then(function () {
                    resolve(true);
                });
            });
        },
        Close: () => {
            Swal.close();
        },
        ConfirmDelete: async (header, message) => {
            return new Promise(function (resolve, reject) {
                Swal.fire({
                    icon: 'question',
                    title: header,
                    text: message,
                    showDenyButton: false,
                    showCancelButton: true,
                    confirmButtonText: 'OK',
                    cancelButtonText: 'Cancel',
                    didOpen: () => { Swal.hideLoading(); },
                }).then((result) => {
                    resolve(result.isConfirmed);
                });
            });
        },
        invokeAsPromise: (promiseHandler, method, args) => {
            console.log(args);

            let promise = null;
            switch (args.length) {
                case 1:
                    promise = window.swal_driver[method](args[0]);
                    break;
                case 2:
                    promise = window.swal_driver[method](args[0], args[1]);
                    break;
                case 3:
                    promise = window.swal_driver[method](args[0], args[1], args[2]);
                    break;
                case 4:
                    promise = window.swal_driver[method](args[0], args[1], args[2], args[3]);
                    break;
                case 5:
                    promise = window.swal_driver[method](args[0], args[1], args[2], args[3], args[4]);
                    break;
                case 6:
                    promise = window.swal_driver[method](args[0], args[1], args[2], args[3], args[4], args[5]);
                    break;
                case 7:
                    promise = window.swal_driver[method](args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                    break;
                default:
                    promise = window.swal_driver[method]();
            }

            // Process the promise
            promise.then((value) => {
                promiseHandler.invokeMethodAsync('SetResult', value);
            }).catch((error) => {
                if (error === undefined) error = "Something bad happened";
                console.log(error);
                promiseHandler.invokeMethodAsync('SetError', error);
            });
        }
    };
})();