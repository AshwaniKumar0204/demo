(function () {
    'use strict';

    angular
        .module('app')
        .controller('PaidPaymentLinkListController', PaidPaymentLinkListController);

    PaidPaymentLinkListController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData', 'loadDataService'];

    function PaidPaymentLinkListController($scope, adminService, Factory, ConstantData, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

        $scope.$watch('formAdmission.$valid', function (IsValid) {
            $scope.IsFormValid = IsValid;
        })

        $scope.getClassStreamTypeList = function () {
            $scope.SubjectOptionOne = null;
            $scope.SubjectOptionTwo = null;
            $scope.ClassStreamTypeId = null;
            $scope.dataLoading = true;
            adminService.getClassStreamTypeList(1, $scope.ClassNo)
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

        //$scope.changeSubjectOptionOne = function () {
        //    $scope.SubjectOptionTwoList = [];
        //    if ($scope.SubjectOptionOne > 0) {
        //        angular.forEach($scope.SubjectOptionOneList, function (subjectOption, key1) {
        //            if (subjectOption.SubjectOptionId != $scope.SubjectOptionOne) {
        //                $scope.SubjectOptionTwoList.push(subjectOption);
        //            }
        //        });
        //    }
        //}

        $scope.getSubjectOptionList = function () {
            if ($scope.ClassStreamTypeId == null || $scope.ClassStreamTypeId == undefined) {
                return;
            }

            $scope.dataLoading = true;
            adminService.getSubjectOptionList($scope.ClassNo, $scope.ClassStreamTypeId, 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SubjectOptionOneList = response.data.SubjectOptionList;
                        //$scope.changeSubjectOptionOne();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
            //$scope.changeSubjectOptionOne();
        }

        $scope.getAcademicSessionList = function () {
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(ConstantData.SessionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getAcademicSessionList();

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

        $scope.getRegistrationFormList = function () {
            $scope.dataLoading = true;
            adminService.getSearchRegistrationFormList(0, $scope.AcademicSessionId, $scope.ClassNo)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.RegistrationFormList = response.data.RegistrationFormList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getRegistrationFormList();

        $scope.changeRegistrationForm = function () {
            $scope.SelectedRegistrationForm = {};
            for (var i = 0; i < $scope.RegistrationFormList.length; i++) {
                if ($scope.RegistrationFormId == $scope.RegistrationFormList[i].RegistrationFormId) {
                    $scope.SelectedRegistrationForm = $scope.RegistrationFormList[i];
                    break;
                }
            }
        }

        $scope.getPaymentLinkList = function () {
            $scope.IsSubmitted = true;
            if (!$scope.IsFormValid) {
                toastr.error("Fill all required fields!!");
                return;
            }

            var obj = {
                AcademicSessionId: $scope.AcademicSessionId,
                ClassNo: $scope.ClassNo,
                RegistrationFormId: $scope.RegistrationFormId,
                ClassStreamTypeId: $scope.ClassStreamTypeId,
                SubjectOptionOne: $scope.SubjectOptionOne,
            }
            $scope.dataLoading = true;
            adminService.getPaidPaymentLinkList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PaymentLinkList = response.data.PaymentLinkList;
                        $scope.AdmissionFeeList = response.data.AdmissionFeeList;
                        $scope.PaymentLinkList = [];
                        $scope.TotalAmount = 0;
                        var IsSameSchool = $('#allow_same_school').val();
                        if (IsSameSchool == undefined || IsSameSchool == null || IsSameSchool == '') {
                            $scope.PaymentLinkList = response.data.PaymentLinkList;
                            angular.forEach(response.data.PaymentLinkList, function (registrationOnline, key1) {
                                $scope.TotalAmount += registrationOnline.PaidAmount;
                            });
                        } else {
                            angular.forEach(response.data.PaymentLinkList, function (registrationOnline, key1) {
                                if (registrationOnline.IsSameSchool == IsSameSchool) {
                                    $scope.PaymentLinkList.push(registrationOnline);
                                    $scope.TotalAmount += registrationOnline.PaidAmount;
                                }
                            });
                        }

                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.editPaymentLink = function (paymentLink) {
            $scope.PaymentLinkDetailList = [];
            $scope.PaymentLink = paymentLink;
            if (paymentLink.ExpiredDate)
                $scope.PaymentLink.ExpiredDate = new Date(paymentLink.ExpiredDate)
            if ($scope.PaymentLink.PaymentLinkDetailList.length > 0)
                $scope.PaymentLinkDetailList = $scope.PaymentLink.PaymentLinkDetailList;
            else {
                angular.forEach($scope.AdmissionFeeList, function (adm, k1) {
                    $scope.PaymentLinkDetailList.push({
                        Amount: adm.Amount,
                        FeeHeadId: adm.FeeHeadId,
                    });
                });
            }
            $('#modal_popUp').modal('show');
        }

        $scope.updatePaymentLink = function () {
            var obj = {
                ExpiredDate: loadDataService.getDateTime($scope.PaymentLink.ExpiredDate),
                PaymentLinkId: $scope.PaymentLink.PaymentLinkId,
                PaymentLinkRegistrationId: $scope.PaymentLink.PaymentLinkRegistrationId,
                PaymentLinkDetailList: $scope.PaymentLinkDetailList,
            }
            $scope.dataLoading = true;
            adminService.updatePaymentLink(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PaymentLink = [];
                        $scope.PaymentLinkDetailList = [];
                        $scope.getPaymentLinkList();
                        $('#modal_popUp').modal('hide');
                        toastr.success("Payment link updated successfully.")
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }


        $scope.tablesToExcel = function (tables, wsnames, wbname, appname) {
            var initailPage = $scope.pageSize;
            $scope.pageSize = $scope.sizeOptions[$scope.sizeOptions.length - 1];
            $scope.IsExporting = true;
            setTimeout(function () {
                tablesToExcel(tables, wsnames, wbname, appname);
            }, 500)
            setTimeout(function () {
                $scope.IsExporting = false;
                $scope.pageSize = initailPage;
                $scope.$apply();
            }, 1000)
        }
    }
})();
