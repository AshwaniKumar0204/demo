(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageRefundController', ManageRefundController);

    ManageRefundController.$inject = ["$scope", "adminService", "loadDataService", "Factory", "ConstantData"];

    function ManageRefundController($scope, adminService, loadDataService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
       
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.RegistrationOnline = {};
        $scope.Refund = {};

        $scope.getRefundStatusList = function () {
            $scope.dataLoading = true;
            adminService.getRefundStatusList(ConstantData.SessionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RefundStatusList = response.data.RefundStatusList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getRefundStatusList();

        $scope.getSearchRegistrationOnlineList = function () {
            $scope.dataLoading = true;
            adminService.getSearchRegistrationOnlineList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RegistrationOnlineList = response.data.RegistrationOnlineList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSearchRegistrationOnlineList();

        $scope.$watch('formRegistrationOnline.$valid', function (value) {
            $scope.isFormValid = value;
        })

        $scope.getClassStreamTypeList = function () {
            $scope.dataLoading = true;
            adminService.getClassStreamTypeList(1, $scope.RegistrationOnline.ClassNo)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassStreamTypeList = response.data.ClassStreamTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.getAcademicSessionList = function () {
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.getAcademicSessionList();

        $scope.getClassListDistinct = function () {
            $scope.dataLoading = true;
            adminService.getClassListDistinct(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassListDistinct = response.data.ClassList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.getClassListDistinct();

        $scope.changeSubjectOptionOne = function () {
            $scope.SubjectOptionTwoList = [];
            if ($scope.RegistrationOnline.SubjectOptionOne > 0) {
                angular.forEach($scope.SubjectOptionOneList, function (subjectOption, key1) {
                    if (subjectOption.SubjectOptionId != $scope.RegistrationOnline.SubjectOptionOne) {
                        $scope.SubjectOptionTwoList.push(subjectOption);
                    }
                });
            }
        }

        $scope.getSubjectOptionList = function () {
            if ($scope.RegistrationOnline.ClassStreamTypeId == null || $scope.RegistrationOnline.ClassStreamTypeId == undefined) {
                return;
            }

            $scope.dataLoading = true;
            adminService.getSubjectOptionList($scope.RegistrationOnline.ClassNo, $scope.RegistrationOnline.ClassStreamTypeId, 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SubjectOptionOneList = response.data.SubjectOptionList;
                        $scope.changeSubjectOptionOne();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
            $scope.changeSubjectOptionOne();
        }

        $scope.afterStudentSelected = function (selected) {
            if (selected != undefined) {
                $scope.dataLoading = true;
                adminService.getRegistrationOnlineDetail(selected.originalObject.RegistrationOnlineId)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.RegistrationOnline = response.data.RegistrationOnline;
                            $scope.getRefundList();
                            $scope.getClassStreamTypeList();
                            $scope.getSubjectOptionList();
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching Registration detail.");
                        $scope.dataLoading = false;
                    })
            } else {
                $scope.SelectedRegistrationOnline = {};
            }
        }

        $scope.getRefundList = function () {
            var obj = {
                RegistrationOnlineId: $scope.RegistrationOnline.RegistrationOnlineId
            }
            $scope.dataLoading = true;
            adminService.getRefundList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.Refund = response.data.RegistrationOnlineList[0];
                        $scope.Refund.AccountNo = parseInt($scope.Refund.AccountNo);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.RegistrationOnline = {};
            $scope.Refund = {};
            document.getElementById('StudentAutoComplete_value').value = null;
            $scope.formRegistrationOnline.$setPristine();
            $scope.formRegistrationOnline.$setUntouched();
        }

        $scope.saveRefund = function () {
            $scope.IsSubmitted = true;
            if ($scope.isFormValid) {
                $scope.Refund.RegistrationOnlineId = $scope.RegistrationOnline.RegistrationOnlineId;
                $scope.dataLoading = true;
                adminService.saveRefund($scope.Refund)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Registration Detail Updated Successfully.")
                            $scope.resetForm();
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error(err.data.Message);
                        $scope.dataLoading = false;
                    })
            } else {
                toastr.error("Fill the all required fields !!");
            }
        }
    }
})();
