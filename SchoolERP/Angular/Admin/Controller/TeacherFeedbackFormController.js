(function () {
    'use strict';

    angular
        .module('app')
        .controller('TeacherFeedbackFormController', TeacherFeedbackFormController);

    TeacherFeedbackFormController.$inject = ["$scope", "adminService", "loadDataService"];

    function TeacherFeedbackFormController($scope, adminService, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.isAlreadySubmitted = false;

        $scope.getTeacherParameterList = function () {
            $scope.dataLoading = true;
            adminService.getTeacherParameterList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ParameterList = response.data.ParameterList;
                        $scope.AcademicSession = response.data.AcademicSession;
                        $scope.AcademicSessionId = response.data.AcademicSession.AcademicSessionId;
                        $scope.AcademicSessionId = 2;
                        $scope.BookReading = response.data.BookReading;
                        $scope.ClassTaught = response.data.ClassTaught;
                        $scope.CoScholastic = response.data.CoScholastic;
                        $scope.TeacherLearningAidsPramaterId = response.data.TeacherLearningAids;
                        $scope.TeachingExprienceId = response.data.TeachingExprienceId;
                        $scope.TrainingProgrammePrameterId = response.data.TrainingProgrammes;
                        $scope.getTeacherSubjectListForFeedback();
                        $scope.getTeachingLearningAidDetail();
                        $scope.isAlreadySubmitted = false;
                    } else if (response.data.Message == 'submitted') {
                        $scope.isAlreadySubmitted = true;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getTeacherParameterList();

        $scope.getBookReadTypeList = function () {
            $scope.dataLoading = true;
            adminService.getBookReadTypeList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.BookReadTypeList = response.data.BookReadTypeList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getBookReadTypeList();

        $scope.getTeacherSubjectListForFeedback = function () {
            $scope.dataLoading = true;
            adminService.getTeacherSubjectListForFeedback($scope.AcademicSessionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.TeacherSubjectList = response.data.TeacherSubjectList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.getTeachingLearningAidDetail = function () {
            $scope.dataLoading = true;
            adminService.getTeachingLearningAidsDetail($scope.AcademicSessionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.TeachingLearningAidList = response.data.TeachingLearningAidsList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        //=====================Teacher Experience======================
        $scope.$watch('formTeacherExperience.$valid', function (value) {
            $scope.IsForm_TE_Valid = value;
        })

        $scope.TeacherExperience = {};
        $scope.TeacherExperienceList = [];
        $scope.addTeacherExperience = function () {
            $scope.Is_TE_Submitted = true;
            if ($scope.TeacherExperience.Year == null || $scope.TeacherExperience.Year == undefined) {
                toastr.error("Year is required!!");
                return;
            }

            if ($scope.TeacherExperience.SchoolName == null || $scope.TeacherExperience.SchoolName == undefined) {
                toastr.error("School is required!!");
                return;
            }
            $scope.TeacherExperienceList.push({ Year: $scope.TeacherExperience.Year, SchoolName: $scope.TeacherExperience.SchoolName });
            $scope.TeacherExperience = {};
            $scope.Is_TE_Submitted = false;
        }
        $scope.removeTeacherExperience = function (index) {
            $scope.TeacherExperienceList.splice(index, 1);
        }

        //=====================Teacher Book Read======================
        $scope.$watch('formTeacherBookRead.$valid', function (value) {
            $scope.IsFromTeacherBookReadValid = value;
        })

        $scope.TeacherBookRead = {};
        $scope.TeacherBookReadList = [];
        $scope.addTeacherBookRead = function () {
            $scope.IsFromTeacherBookReadSubmitted = true;
            if ($scope.TeacherBookRead.BookReadTypeDetail == null || $scope.TeacherBookRead.BookReadTypeDetail == undefined) {
                toastr.error("Book Type is required!!");
                return;
            }

            if ($scope.TeacherBookRead.BookName == null || $scope.TeacherBookRead.BookName == undefined) {
                toastr.error("Book is required!!");
                return;
            }

            if ($scope.TeacherBookRead.Author == null || $scope.TeacherBookRead.Author == undefined) {
                toastr.error("Author is required!!");
                return;
            }
            $scope.TeacherBookReadList.push({
                BookReadType: $scope.TeacherBookRead.BookReadTypeDetail.Key,
                BookReadTypeName: $scope.TeacherBookRead.BookReadTypeDetail.Value,
                BookName: $scope.TeacherBookRead.BookName,
                Author: $scope.TeacherBookRead.Author
            });
            $scope.TeacherBookRead = {};
            $scope.IsFromTeacherBookReadSubmitted = false;
        }
        $scope.removeTeacherBookRead = function (index) {
            $scope.TeacherBookReadList.splice(index, 1);
        }

        //=====================Teacher Coscholastic======================
        $scope.$watch('formTeacherCoScholastic.$valid', function (value) {
            $scope.IsForm_TE_Valid = value;
        })

        $scope.TeacherCoScholastic = {};
        $scope.TeacherCoScholasticList = [];
        $scope.addTeacherCoScholastic = function () {
            $scope.Is_CS_Submitted = true;
            if ($scope.TeacherCoScholastic.Events == null || $scope.TeacherCoScholastic.Events == undefined) {
                toastr.error("Events is required!!");
                return;
            }

            if ($scope.TeacherCoScholastic.EventDate == null || $scope.TeacherCoScholastic.EventDate == undefined) {
                toastr.error("Event Date is required!!");
                return;
            }

            if ($scope.TeacherCoScholastic.TeacherRole == null || $scope.TeacherCoScholastic.TeacherRole == undefined) {
                toastr.error("Teacher Role is required!!");
                return;
            }
            $scope.TeacherCoScholasticList.push({
                Events: $scope.TeacherCoScholastic.Events,
                Events: $scope.TeacherCoScholastic.Events,
                EventDate: $scope.TeacherCoScholastic.EventDate,
                TeacherRole: $scope.TeacherCoScholastic.TeacherRole,
            });
            $scope.TeacherCoScholastic = {};
            $scope.Is_CS_Submitted = false;
        }
        $scope.removeTeacherCoScholastic = function (index) {
            $scope.TeacherCoScholasticList.splice(index, 1);
        }

        //=====================Teacher Programmes======================
        $scope.$watch('formTrainingProgramme.$valid', function (value) {
            $scope.IsForm_TE_Valid = value;
        })

        $scope.TrainingProgramme = {};
        $scope.TrainingProgrammeList = [];
        $scope.addTrainingProgramme = function () {
            $scope.IsTrainingProgrammeSubmitted = true;
            if ($scope.TrainingProgramme.Subject == null || $scope.TrainingProgramme.Subject == undefined) {
                toastr.error("Subject is required!!");
                return;
            }

            if ($scope.TrainingProgramme.TrainingDate == null || $scope.TrainingProgramme.TrainingDate == undefined) {
                toastr.error("Training Date is required!!");
                return;
            }

            if ($scope.TrainingProgramme.Place == null || $scope.TrainingProgramme.Place == undefined) {
                toastr.error("Place is required!!");
                return;
            }
            $scope.TrainingProgrammeList.push({
                Subject: $scope.TrainingProgramme.Subject,
                Subject: $scope.TrainingProgramme.Subject,
                TrainingDate: $scope.TrainingProgramme.TrainingDate,
                Place: $scope.TrainingProgramme.Place,
            });
            $scope.TrainingProgramme = {};
            $scope.IsTrainingProgrammeSubmitted = false;
        }
        $scope.removeTrainingProgramme = function (index) {
            $scope.TrainingProgrammeList.splice(index, 1);
        }

        $scope.saveTeacherPerformance = function () {
            var PerformanceDetailList = [];
            //for (var i = 0; i < $scope.ParameterList.length; i++) {
            //    for (var j = 0; j < $scope.ParameterList[i].ChildPrameterList.length; j++) {
            //        var x2 = $scope.ParameterList[i].ChildPrameterList[j];
            //        if (x2.Remarks)
            //            PerformanceDetailList.push(x2);
            //        else {
            //            toastr.error(x2.ParameterName + " is required.");
            //            return;
            //        }
            //    }
            //}

            angular.forEach($scope.TeacherCoScholasticList, function (x1) {
                x1.EventDate = loadDataService.getDateTime(x1.EventDate);
            })

            angular.forEach($scope.TrainingProgrammeList, function (x1) {
                x1.TrainingDate = loadDataService.getDateTime(x1.TrainingDate);
            })

            angular.forEach($scope.ParameterList, function (x1) {
                angular.forEach(x1.ChildPrameterList, function (x2) {
                    PerformanceDetailList.push({ ParameterId: x2.ParameterId, Remarks: x2.Remarks });
                })
            })

            var obj = {
                AcademicSessionId: $scope.AcademicSessionId,
                PerformanceParameterList: PerformanceDetailList,
                TeacherExperienceList: $scope.TeacherExperienceList,
                TeacherCoScholasticList: $scope.TeacherCoScholasticList,
                TrainingProgrammeList: $scope.TrainingProgrammeList,
                TeachingLearningAidList: $scope.TeachingLearningAidList,
                TeacherBookReadList: $scope.TeacherBookReadList,
            }
            $scope.dataLoading = true;
            adminService.saveTeacherPerformance(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Data Updated Successfully.");
                        $scope.ParameterList = [];
                        $scope.TeacherExperienceList = [];
                        $scope.TeacherCoScholasticList = [];
                        $scope.TrainingProgrammeList = [];
                        $scope.TeachingLearningAidList = [];
                        $scope.TeacherBookReadList = [];
                        $scope.isAlreadySubmitted = true;
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
