(function () {
    'use strict';

    angular
        .module('app')
        .controller('GenerateAdmissionPaymentLinkController', GenerateAdmissionPaymentLinkController);

    GenerateAdmissionPaymentLinkController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData', '$location'];

    function GenerateAdmissionPaymentLinkController($scope, adminService, Factory, ConstantData, $location) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };

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

        $scope.getResultTypeList = function () {
            $scope.dataLoading = true;
            adminService.getResultTypeList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ResultTypeList = response.data.ResultTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getResultTypeList();

        $scope.resetRegistrationOnlineList = function () {
            $scope.RegistrationOnlineList = [];
        }

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

        $scope.getSubjectOptionList = function () {
            if ($scope.ClassStreamTypeId == null || $scope.ClassStreamTypeId == undefined) {
                return;
            }

            $scope.dataLoading = true;
            adminService.getSubjectOptionList($scope.ClassNo, $scope.ClassStreamTypeId, 0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SubjectOptionOneList = response.data.SubjectOptionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.getRegistrationOnlineListForGenerateLink = function () {
            $scope.IsSubmitted = true;
            if (!$scope.IsFormValid) {
                toastr.error("Fill all required fields!!");
                return;
            }
            var IsSameSchool = $('#allow_same_school').val();
            if (IsSameSchool == 0)
                $scope.ResultType = 1;
            var obj = {
                AcademicSessionId: $scope.AcademicSessionId,
                ClassNo: $scope.ClassNo,
                RegistrationFormId: $scope.RegistrationFormId,
                ClassStreamTypeId: $scope.ClassStreamTypeId,
                SubjectOptionOne: $scope.SubjectOptionOne,
                SubjectOptionTwo: $scope.SubjectOptionTwo,
                ResultType: $scope.ResultType,
                RegistrationOnlineStatus: 2,
            }
            $scope.dataLoading = true;
            adminService.getRegistrationOnlineListForGenerateLink(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        //$scope.RegistrationOnlineList = response.data.RegistrationOnlineList;
                        //console.log(response.data.RegistrationOnlineList);
                        $scope.RegistrationOnlineList = [];

                        angular.forEach(response.data.RegistrationOnlineList, function (registrationOnline, key1) {
                            registrationOnline.IsChecked = true;
                            if (registrationOnline.IsSameSchool == IsSameSchool && (IsSameSchool == 1 || IsSameSchool == 0)) {
                                $scope.RegistrationOnlineList.push(registrationOnline);
                            }
                        });

                        if (IsSameSchool == undefined || IsSameSchool == null || IsSameSchool == '') {
                            $scope.RegistrationOnlineList = response.data.RegistrationOnlineList;
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

        $scope.CheckedAll = function () {
            var IsCheckedAll = document.getElementById('IsCheckedAll').checked;
            console.log(IsCheckedAll);
            angular.forEach($scope.RegistrationOnlineList, function (registrationOnline, key1) {
                if (IsCheckedAll)
                    registrationOnline.IsChecked = true;
                else
                    registrationOnline.IsChecked = false;
            })
        }

        $scope.changeChecked = function (registrationOnline) {
            registrationOnline.IsChecked = !registrationOnline.IsChecked;
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

        $scope.getAdmissionFeeListForLink = function () {
            $scope.dataLoading = true;
            adminService.getAdmissionFeeListForLink($scope.AcademicSessionId, $scope.ClassNo, $scope.RegistrationFormId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AdmissionFeeList = response.data.AdmissionFeeList;
                        $('#modal_popUp').modal('show');
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.RegistrationOnlineFeeList = [];
        $scope.previewPaymentLink = function () {
            $scope.getAdmissionFeeListForLink();
            $scope.RegistrationOnlineFeeList = [];
            angular.forEach($scope.RegistrationOnlineList, function (registrationOnline, key1) {
                if (registrationOnline.IsChecked) {
                    var obj = {
                        RegistrationOnlineId: registrationOnline.RegistrationOnlineId,
                        RegistrationNo: registrationOnline.RegistrationNo,
                        StudentName: registrationOnline.StudentName,
                        FathersName: registrationOnline.FathersName,
                        IsChecked: registrationOnline.IsChecked,
                    }
                    $scope.RegistrationOnlineFeeList.push(obj);
                }
            });
        }

        $scope.savePaymentLink = function () {
            var RegistrationOnlineIds = [];
            angular.forEach($scope.RegistrationOnlineFeeList, function (registration, key1) {
                if (registration.IsChecked)
                    RegistrationOnlineIds.push(registration.RegistrationOnlineId);
            })

            if (RegistrationOnlineIds.length == 0) {
                toastr.error("No Student is Selected");
                return;
            }

            var obj = {
                RegistrationOnlineIds: RegistrationOnlineIds,
                ExpiredDate: $scope.ExpiredDate,
                AcademicSessionId: $scope.AcademicSessionId,
                ClassNo: $scope.ClassNo,
                AdmissionFeeList: $scope.AdmissionFeeList,
            }
            console.log(obj);
            $scope.dataLoading = true;
            adminService.savePaymentLink(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Link Generated Successfully.");
                        $scope.RegistrationOnlineList = [];
                        $scope.RegistrationOnlineFeeList = [];
                        $scope.ExpiredDate = null;
                        $scope.AdmissionFeeList = [];
                        //$scope.AcademicSessionId = null;
                        $scope.ClassNo = null;
                        $scope.RegistrationFormId = null;
                        $('#modal_popUp').modal('hide');
                        $scope.formAdmission.$setPristine();
                        $scope.formAdmission.$setUntouched();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
    }
})();
