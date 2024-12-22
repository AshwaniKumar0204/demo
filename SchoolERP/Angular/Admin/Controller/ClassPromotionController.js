(function () {
    'use strict';

    angular
        .module('app')
        .controller('ClassPromotionController', ClassPromotionController);

    ClassPromotionController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData'];

    function ClassPromotionController($scope, adminService, Factory, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.PreviousClassId = null;
        $scope.PreviousSectionId = null;
        $scope.NewClassId = null;
        $scope.NewSectionId = null;
        $scope.FromAcademicSessionId = null;
        $scope.ToAcademicSessionId = null;
        $scope.AdmissionList = [];
        $scope.StudentPromotionList = [];
        $scope.PromotionStatusList = ConstantData.PromotionStatus;

        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.$watch('formClassPromotion.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.getAcademicSessionList = function () {
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(ConstantData.SessionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                        var index = { AcademicSessionId: null, SessionName: '--Select Session--' };
                        $scope.AcademicSessionList.unshift(index);
                        $scope.ToAcademicSessionList = $scope.AcademicSessionList;
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
            adminService.getClassList(ConstantData.ClassStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                        var index = { ClassId: null, ClassName: '--Select Class--' };
                        $scope.ClassList.unshift(index);
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

        $scope.changePreviousClass = function () {
            $scope.PreviousSectionList = [];
            angular.forEach($scope.MainSectionList, function (section, key) {
                if (section.ClassId == $scope.PreviousClassId) {
                    $scope.PreviousSectionList.push(section);
                }
            })
            var index = { SectionId: null, SectionName: '--Select Previous Section--' };
            $scope.PreviousSectionList.unshift(index);
            $scope.PreviousSectionId = null;

            if ($scope.PreviousClassId != null) {
                for (var i = 0; i < $scope.ClassList.length; i++) {
                    var classModel = $scope.ClassList[i];
                    if (classModel.ClassId > $scope.PreviousClassId) {
                        $scope.NewClassId = classModel.ClassId;
                        $scope.changeNewClass();
                        break;
                    }
                }
            }

        }

        $scope.changeNewClass = function () {
            $scope.NewSectionList = [];
            $scope.NewSectionListWithoutIndex = [];
            angular.forEach($scope.MainSectionList, function (section, key) {
                if (section.ClassId == $scope.NewClassId) {
                    $scope.NewSectionList.push(section);
                    $scope.NewSectionListWithoutIndex.push(section);
                }
            })
            var index = { SectionId: null, SectionName: '--Select New Section--' };
            $scope.NewSectionList.unshift(index);
            $scope.NewSectionId = null;
        }

        $scope.changeFromSession = function () {
            $scope.ToAcademicSessionList = [];
            angular.forEach($scope.AcademicSessionList, function (session, key) {
                if (session.AcademicSessionId > $scope.FromAcademicSessionId) {
                    $scope.ToAcademicSessionList.push(session);
                }
            })
            var index = { AcademicSessionId: null, SessionName: '--Select New Session--' };
            $scope.ToAcademicSessionList.unshift(index);
        }

        $scope.getSectionList = function () {
            $scope.dataLoading = true;
            adminService.getSectionList(ConstantData.SectionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainSectionList = response.data.SectionList;
                        $scope.changePreviousClass();
                        $scope.changeNewClass();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSectionList();

        $scope.getStudentPromatableList = function () {
            $scope.IsSubmitted = true;
            if (!$scope.IsFormValid) {
                toastr.error("Fill all required fields!!", "Validation Error");
                return;
            }
            $scope.dataLoading = true;
            adminService.getStudentPromatableList($scope.PreviousSectionId, $scope.FromAcademicSessionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AdmissionList = response.data.AdmissionList;
                        angular.forEach($scope.AdmissionList, function (admission, key) {
                            admission.NewSectionId = $scope.NewSectionId;
                            admission.NewRollNo = key + 1;
                        })
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.verifyStudentPromotionList = function () {
            $scope.StudentPromotionList = [];
            for (var i = 0; i < $scope.AdmissionList.length; i++) {
                var admission = $scope.AdmissionList[i];
                if (admission.IsSelected) {
                    if (admission.NewRollNo > 0) {

                    } else {
                        toastr.error("Invalid Value in New Roll No!!", "Roll No Error");
                        return;
                    }
                }
            }
            var SaveClassPromotionModel = {
                OldSectionId: $scope.PreviousSectionId,
                StudentPromotionList: $scope.AdmissionList
            }
            $scope.dataLoading = true;
            adminService.veriyfyClassPromotion(SaveClassPromotionModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StudentPromotionList = response.data.StudentPromotionList;
                        $scope.ArrearFeeHeadId = response.data.ArrearFeeHeadId;
                        $scope.IsVerify = true;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.SaveClassPromotion = function () {
            var SaveClassPromotionModel = {
                OldSectionId: $scope.PreviousSectionId,
                NewAcademicSessionId: $scope.ToAcademicSessionId,
                StudentPromotionList: $scope.StudentPromotionList,
                ArrearFeeHeadId: $scope.ArrearFeeHeadId,
            }
            $scope.dataLoading = true;
            adminService.SaveClassPromotion(SaveClassPromotionModel)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Student Promotion Details Updated Sucessfully", "Promotion Successful");
                        $scope.resetFormClassPromotion();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.resetFormClassPromotion = function () {
            //$scope.PreviousClassId = null;
            //$scope.PreviousSectionId = null;
            //$scope.NewClassId = null;
            //$scope.NewSectionId = null;
            //$scope.FromAcademicSessionId = null;
            //$scope.ToAcademicSessionId = null;
            //$scope.changeFromSession();
            //$scope.changeNewClass();
            //$scope.changePreviousClass();
            $scope.AdmissionList = [];
            $scope.StudentPromotionList = [];
            $scope.IsVerify = false;
            $scope.AdmissionList = [];
            $scope.StudentPromotionList = [];
            $scope.ArrearFeeHeadId = null;
            $scope.IsSubmitted = false;
            $scope.formClassPromotion.$setPristine();
            $scope.formClassPromotion.$setUntouched();
        }

    }
})();
