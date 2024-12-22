(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageBookSubjectRackController', ManageBookSubjectRackController);

    ManageBookSubjectRackController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageBookSubjectRackController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.BookSubjectRack = {};
        $scope.getBookSubjectList = function () {
            $scope.dataLoading = true;
            adminService.getBookSubjectList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookSubjectList = response.data.BookSubjectList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getBookSubjectList();

        $scope.getRackList = function () {
            $scope.dataLoading = true;
            adminService.getRackList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RackList = response.data.RackList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getRackList();

        $scope.afterRackSelected = function (obj) {
            if (obj != undefined) {
                $scope.BookSubjectRack.RackId = obj.originalObject.RackId;
            } else {
                $scope.BookSubjectRack.RackId = null;
            }
        }

        $scope.getBookSubjectRackList = function () {
            $scope.dataLoading = true;
            adminService.getBookSubjectRackList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookSubjectRackList = response.data.BookSubjectRackList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getBookSubjectRackList();

        $scope.afterSubjectSelected = function (obj) {
            if (obj != undefined) {
                $scope.BookSubjectRack.BookSubjectId = obj.originalObject.BookSubjectId;
            } else {
                $scope.BookSubjectRack.BookSubjectId = null;
            }
        }

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassListDistinct(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getClassList();

        $scope.$watch('formBookSubjectRack.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveBookSubjectRack = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                if ($scope.BookSubjectRack.BookSubjectId == null || $scope.BookSubjectRack.BookSubjectId == undefined) {
                    toastr.error("Subject is required!!");
                    return;
                }

                if ($scope.BookSubjectRack.RackId == null || $scope.BookSubjectRack.RackId == undefined) {
                    toastr.error("Rack is required!!");
                    return;
                }

                var BookSubjectRackModel = {
                    BookSubjectRackId: $scope.BookSubjectRack.BookSubjectRackId,
                    BookSubjectId: $scope.BookSubjectRack.BookSubjectId,
                    RackId: $scope.BookSubjectRack.RackId,
                    ClassNo: $scope.BookSubjectRack.ClassNo,
                }

                $scope.dataLoading = true;
                adminService.saveBookSubjectRack(BookSubjectRackModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (BookSubjectRackModel.BookSubjectRackId > 0)
                                toastr.success("Subject Rack Updated Successfully.");
                            else
                                toastr.success("New Subject Rack Created Successfully.");
                            $scope.resetForm();
                            $scope.getBookSubjectRackList();
                            $('#modal_Student_Charge').modal('hide');
                        } else {
                            toastr.error(response.data.Message);
                            $scope.dataLoading = false;
                        }
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
        }

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.BookSubjectRack = {};
            document.getElementById('SubjectAnguComplete_value').value = "";
            document.getElementById('RackAnguComplete_value').value = "";
            $scope.formBookSubjectRack.$setPristine();
            $scope.formBookSubjectRack.$setUntouched();
        }

        $scope.newBookSubjectRack = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editBookSubjectRack = function (BookSubjectRackModel) {
            $scope.resetForm();
            $scope.BookSubjectRack = BookSubjectRackModel;
            document.getElementById('SubjectAnguComplete_value').value = $scope.BookSubjectRack.SubjectName;
            document.getElementById('RackAnguComplete_value').value = $scope.BookSubjectRack.RackName;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (BookSubjectRackId) {
            $scope.BookSubjectRackId = BookSubjectRackId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteBookSubjectRack = function () {
            $scope.dataLoading = true;
            adminService.deleteBookSubjectRack($scope.BookSubjectRackId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Subject Rack deleted successfully.", "Deleted Successfully.")
                        $scope.BookSubjectRackId = null;
                        $scope.getBookSubjectRackList();
                        $('#modal_confirmation').modal('hide');
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
    }
})();
