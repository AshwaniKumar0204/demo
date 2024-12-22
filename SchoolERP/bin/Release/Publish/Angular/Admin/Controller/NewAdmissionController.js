(function () {
    "use strict";

    angular.module("app").controller("NewAdmissionController", NewAdmissionController);

    NewAdmissionController.$inject = ["$scope", "adminService", "$location", "loadDataService", "ConstantData"];

    function NewAdmissionController($scope, adminService, $location, loadDataService, ConstantData) {
        $scope.checkSysytemLogin();
        $scope.GenderList = ConstantData.Gender;
        $scope.NationalityList = ConstantData.Nationality;
        $scope.ReligionList = ConstantData.Religion;
        $scope.CategoryList = ConstantData.Category;
        $scope.BloodGroupList = ConstantData.BloodGroup;
        $scope.StaffWardList = ConstantData.StaffWard;
        $scope.StateList = [];

        $scope.resetAdmission = function () {
            $scope.Admission = {};
            $scope.Admission.AdmissionType = 1;
            $scope.Admission.StaffWard = false;
            $scope.Admission.SectionId = null;
            $scope.Admission.Staff = null;
            $scope.Admission.StudentTypeId = null;
            $scope.Admission.Gender = null;
            $scope.Admission.Nationality = null;
            $scope.Admission.Religion = null;
            $scope.Admission.Category = null;
            $scope.Admission.BloodGroup = null;
            $scope.Admission.ClassId = null;
            $scope.Admission.AcademicSessionId = null;
            $scope.Admission.HouseId = null;
            $scope.Admission.PermanentCityId = null;
            $scope.Admission.PermanentStateId = null;
            $scope.Admission.CorrespondenceCityId = null;
            $scope.Admission.CorrespondenceStateId = null;
            $scope.Admission.SameAddress = false;
            $scope.Admission.AdmissionDate = new Date();
            $scope.AvailableStudents = 0;
            $scope.IsSubmitted = false;
            $scope.IsSubjectCombinationSubmitted = false;
            $scope.IsTransportSubmitted = false;
        }
        $scope.resetAdmission();


        $scope.generateAdmissionNo = function () {
            $scope.dataLoading = true;
            adminService.GenerateAdmissionNo($scope.Admission.AcademicSessionId, $scope.Admission.SectionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.Admission.AdmissionNo = response.data.AdmissionNo;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.$watch('formAdmission.$valid', function (value) {
            $scope.IsFormValid = value;
        })


        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(ConstantData.ClassStatus[1].Key)
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

        $scope.SubjectCombinationList = [];
        $scope.changeClass = function () {
            $scope.getAvailableStudents();
            $scope.getAcademicSessionList();
            $scope.SectionList = [];
            angular.forEach($scope.MainSectionList, function (section, key) {
                if (section.ClassId == $scope.Admission.ClassId) {
                    $scope.SectionList.push(section);
                }
            })
            //var index = { SectionId: null, SectionName: '--Select Section--' };
            //$scope.SectionList.unshift(index);

            $scope.SubjectCombinationList = [];
            angular.forEach($scope.MainSubjectCombinationList, function (subject, key1) {
                if (subject.ClassId == $scope.Admission.ClassId) {
                    $scope.SubjectCombinationList.push(subject);
                }
            })
        }

        $scope.getAvailableStudents = function () {
            if ($scope.Admission.ClassId != null && $scope.Admission.AcademicSessionId != null && $scope.Admission.SectionId != null) {
                $scope.dataLoading = true;
                adminService.getAvailableStudents($scope.Admission.AcademicSessionId, $scope.Admission.SectionId)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            $scope.AvailableStudents = response.data.AvailableStudents;
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    });
                $scope.generateAdmissionNo();
            } else {
                $scope.AvailableStudents = 0
            }
        }

        $scope.changeCorrespondenceState = function () {
            $scope.CorrespondenceCityList = [];
            angular.forEach($scope.CityList, function (city, key) {
                if (city.StateId == $scope.Admission.CorrespondenceStateId) {
                    $scope.CorrespondenceCityList.push(city);
                }
            })
            var index = { CityId: null, CityName: '--Select City--' };
            $scope.CorrespondenceCityList.unshift(index);
            $scope.sameAsCorrespondenceAddress();
        }

        $scope.changePermanentState = function () {
            $scope.PermanentCityList = [];
            angular.forEach($scope.CityList, function (city, key) {
                if (city.StateId == $scope.Admission.PermanentStateId) {
                    $scope.PermanentCityList.push(city);
                }
            })
            var index = { CityId: null, CityName: '--Select City--' };
            $scope.PermanentCityList.unshift(index);
        }

        $scope.getCityList = function () {
            $scope.dataLoading = true;
            adminService.getCityList(ConstantData.CityStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.CityList = response.data.CityList;
                        $scope.changeCorrespondenceState();
                        $scope.changePermanentState();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getCityList();

        $scope.getStateList = function () {
            $scope.dataLoading = true;
            adminService.getStateList(ConstantData.StateStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StateList = response.data.StateList;
                        var index = { StateId: null, StateName: '--Select State--' };
                        $scope.StateList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStateList();

        $scope.getHouseList = function () {
            $scope.dataLoading = true;
            adminService.getHouseList(ConstantData.HouseStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.HouseList = response.data.HouseList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getHouseList();

        $scope.getAcademicSessionList = function () {
            if ($scope.Admission.ClassId == null || $scope.Admission.ClassId == undefined) {
                $scope.AcademicSessionList = [];
                $scope.AcademicSessionId = null;
                return;
            }
            var obj = {
                ClassId: $scope.Admission.ClassId,
                Status: 1
            }
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(obj)
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

        $scope.getSectionList = function () {
            $scope.dataLoading = true;
            adminService.getSectionList(ConstantData.SectionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainSectionList = response.data.SectionList;
                        $scope.changeClass();
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


        $scope.getStudentTypeList = function () {
            $scope.dataLoading = true;
            adminService.getStudentTypeList(ConstantData.StudentTypeStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StudentTypeList = response.data.StudentTypeList;
                        var index = { StudentTypeId: null, TypeName: '--Select Student Type--' };
                        $scope.StudentTypeList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStudentTypeList();

        //$scope.sameAsPermanentAddress = function () {
        //    if (document.getElementById('IsSameAddress').checked) {
        //        $scope.Admission.CorrespondenceStateId = $scope.Admission.PermanentStateId;
        //        $scope.changeCorrespondenceState();
        //        $scope.Admission.CorrespondenceCityId = $scope.Admission.PermanentCityId;
        //        $scope.Admission.CorrespondenceAddressLine1 = $scope.Admission.PermanentAddressLine1;
        //        $scope.Admission.CorrespondenceAddressLine2 = $scope.Admission.PermanentAddressLine2;
        //    }
        //}

        $scope.sameAsCorrespondenceAddress = function () {
            if (document.getElementById('IsSameAddress').checked) {
                $scope.Admission.PermanentStateId = $scope.Admission.CorrespondenceStateId;
                $scope.changePermanentState();
                $scope.Admission.PermanentCityId = $scope.Admission.CorrespondenceCityId;
                $scope.Admission.PermanentAddressLine1 = $scope.Admission.CorrespondenceAddressLine1;
                $scope.Admission.PermanentAddressLine2 = $scope.Admission.CorrespondenceAddressLine2;
                $scope.Admission.PermanentPinCode = $scope.Admission.CorrespondecePinCode;
            }
        }

        $scope.newAdmission = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                if ($scope.StudentSubjectCombination.SubjectCombinationId > 0) {
                    $scope.IsSubjectCombinationSubmitted = true;
                    if (!$scope.IsFormSubjectCombinationValid) {
                        toastr.error("Fill all required fields of subject combination");
                        return;
                    }
                } else {
                    $scope.IsSubjectCombinationSubmitted = false;
                    $scope.formStudentSubjectCombination.$setPristine();
                    $scope.formStudentSubjectCombination.$setUntouched();
                }

                if ($scope.Transport.TransportGroupId > 0) {
                    $scope.IsTransportSubmitted = true;
                    if (!$scope.IsFormTransportValid) {
                        toastr.error("Fill all required fields of Transport");
                        return;
                    }
                } else {
                    $scope.IsTransportSubmitted = false;
                    $scope.formTransport.$setPristine();
                    $scope.formTransport.$setUntouched();
                }

                var AdmissionModel = {
                    AcademicSessionId: $scope.Admission.AcademicSessionId,
                    ClassId: $scope.Admission.ClassId,
                    AdmissionDate: loadDataService.getDateTime($scope.Admission.AdmissionDate),
                    SectionId: $scope.Admission.SectionId,
                    StudentRegistrationId: $scope.StudentRegistrationId,
                    AdmissionType: document.getElementById('AdmissionType').value,
                    StaffWard: $scope.Admission.StaffWard,
                    Student: {
                        StudentTypeId: $scope.Admission.StudentTypeId,
                        AadhaarNo: $scope.Admission.AadhaarNo,
                        AlternateNo: $scope.Admission.AlternateNo,
                        Category: $scope.Admission.Category,
                        PermanentAddressLine1: $scope.Admission.PermanentAddressLine1,
                        PermanentAddressLine2: $scope.Admission.PermanentAddressLine2,
                        PermanentCityId: $scope.Admission.PermanentCityId,
                        HouseName: $scope.Admission.HouseName,
                        CorrespondenceAddressLine1: $scope.Admission.CorrespondenceAddressLine1,
                        CorrespondenceAddressLine2: $scope.Admission.CorrespondenceAddressLine2,
                        CorrespondenceCityId: $scope.Admission.CorrespondenceCityId,
                        DateOfBirth: loadDataService.getDateTime($scope.Admission.DateOfBirth),
                        Email: $scope.Admission.Email,
                        FamilyAnnualIncome: $scope.Admission.FamilyAnnualIncome,
                        FatherName: $scope.Admission.FatherName,
                        FatherOccupation: $scope.Admission.FatherOccupation,
                        FatherAadhaarNo: $scope.Admission.FatherAadhaarNo,
                        FullName: $scope.Admission.FullName,
                        Gender: $scope.Admission.Gender,
                        MobileNo: $scope.Admission.MobileNo,
                        MotherName: $scope.Admission.MotherName,
                        MotherOccupation: $scope.Admission.MotherOccupation,
                        MotherAadharNo: $scope.Admission.MotherAadharNo,
                        Nationality: $scope.Admission.Nationality,
                        RegistrationDate: $scope.Admission.RegistrationDate,
                        Religion: $scope.Admission.Religion,
                        StudentStatus: $scope.Admission.StudentStatus,
                        AdmissionNo: $scope.Admission.AdmissionNo,
                        BloodGroup: $scope.Admission.BloodGroup,
                        HouseId: $scope.Admission.HouseId,
                        Remarks: $scope.Admission.Remarks,
                        PermanentPinCode: $scope.Admission.PermanentPinCode,
                        CorrespondecePinCode: $scope.Admission.CorrespondecePinCode,
                        PreviousSchoolName: $scope.Admission.PreviousSchoolName,
                        PreviousBoardName: $scope.Admission.PreviousBoardName,
                        LastClassAttend: $scope.Admission.LastClassAttend,
                        MediumOfInstruction: $scope.Admission.MediumOfInstruction,
                        PreviousSchoolTCNo: $scope.Admission.PreviousSchoolTCNo,
                        PreviousSchoolTCIssueDate: loadDataService.getDateTime($scope.Admission.PreviousSchoolTCIssueDate),
                        RegistrationOnlineId: $scope.RegistrationOnline.RegistrationOnlineId
                    },
                    StudentTransport: {
                        StudentTransportId: $scope.Transport.StudentTransportId,
                        TransportGroupId: $scope.Transport.TransportGroupId,
                        BusId: $scope.Transport.BusId,
                        PickupPoint: $scope.Transport.PickupPoint,
                        PickupTime: loadDataService.getTime($scope.Transport.PickupTime),
                        StartDate: loadDataService.getDateTime($scope.Transport.StartDate),
                        EndDate: loadDataService.getDateTime($scope.Transport.EndDate),
                        Status: $scope.Transport.Status,
                    },
                    StudentSubjectCombination: {
                        AdmissionId: $scope.StudentSubjectCombination.AdmissionId,
                        StudentSubjectCombinationId: $scope.StudentSubjectCombination.StudentSubjectCombinationId,
                        SubjectCombinationId: $scope.StudentSubjectCombination.SubjectCombinationId,
                        StartDate: loadDataService.getDateTime($scope.StudentSubjectCombination.StartDate),
                        EndDate: loadDataService.getDateTime($scope.StudentSubjectCombination.EndDate)
                    }

                }

                $scope.dataLoading = true;
                adminService.newAdmission(AdmissionModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("Student Admission Successfully Done.");
                            $scope.resetForm();
                            sessionStorage.setItem('admissionId', response.data.AdmissionId);
                            $location.path('StudentPhotoUpdate');
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
        }

        $scope.resetForm = function () {
            $scope.resetAdmission();
            $scope.changeCorrespondenceState();
            $scope.changePermanentState();
            $scope.formAdmission.$setPristine();
            $scope.formAdmission.$setUntouched();
            $scope.formTransport.$setPristine();
            $scope.formTransport.$setUntouched();
            $scope.formStudentSubjectCombination.$setPristine();
            $scope.formStudentSubjectCombination.$setUntouched();
        }

        //Transport
        $scope.Transport = {};
        $scope.getGroupList = function () {
            $scope.dataLoading = true;
            adminService.getTransportGroupList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.groups = response.data.TransportGroups;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.getGroupList();

        $scope.GetBusList = function () {
            $scope.dataLoading = true;
            adminService.getBusList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.buses = response.data.Buses;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error(err.data.Message);
                    $scope.dataLoading = false;
                })
        }
        $scope.GetBusList();

        $scope.$watch('formTransport.$valid', function (value) {
            $scope.IsFormTransportValid = value;
        })


        //Subject Combination
        $scope.StudentSubjectCombination = {};
        $scope.$watch('formStudentSubjectCombination.$valid', function (value) {
            $scope.IsFormSubjectCombinationValid = value;
        })

        $scope.getSubjectCombinationList = function () {
            $scope.dataLoading = true;
            adminService.getSubjectCombinationList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainSubjectCombinationList = response.data.SubjectCombinationList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSubjectCombinationList();

        $scope.RegistrationOnline = {};
        $scope.checkRegistrationSession = function () {
            if (sessionStorage.getItem('studentRegistrationDetail') != null && sessionStorage.getItem('studentRegistrationDetail') != undefined) {
                var StudentRegistration = JSON.parse(sessionStorage.getItem('studentRegistrationDetail'));
                $scope.Admission.FullName = StudentRegistration.Name;
                $scope.Admission.PermanentStateId = StudentRegistration.StateId;
                $scope.changePermanentState();
                $scope.Admission.PermanentCityId = StudentRegistration.PermanentCityId;
                $scope.Admission.PermanentAddressLine1 = StudentRegistration.Address;
                $scope.Admission.StudentTypeId = StudentRegistration.StudentTypeId;
                $scope.Admission.ClassId = StudentRegistration.ClassId;
                $scope.changeClass();
                $scope.Admission.AcademicSessionId = StudentRegistration.AcademicSessionId;
                $scope.Admission.FatherName = StudentRegistration.FatherName;
                $scope.Admission.MotherName = StudentRegistration.MotherName;
                $scope.Admission.Gender = StudentRegistration.Gender;
                $scope.Admission.Email = StudentRegistration.EmailId;
                $scope.Admission.DateOfBirth = new Date(StudentRegistration.DOB);
                $scope.Admission.MobileNo = parseInt(StudentRegistration.MobileNo);
                $scope.Admission.AadhaarNo = parseInt(StudentRegistration.AadharNo);
                $scope.StudentRegistrationId = StudentRegistration.StudentRegistrationId;
                $scope.Transport.PickupPoint = StudentRegistration.Address;
                sessionStorage.removeItem('studentRegistrationDetail');
            } else if (sessionStorage.getItem('registrationOnlineDetail') != null && sessionStorage.getItem('registrationOnlineDetail') != undefined) {
                var StudentRegistration = JSON.parse(sessionStorage.getItem('registrationOnlineDetail'));
                $scope.RegistrationOnline = StudentRegistration;
                $scope.Admission.FullName = StudentRegistration.StudentName;
                $scope.Admission.CorrespondenceAddressLine1 = StudentRegistration.FatherPresentAddress;
                $scope.Admission.PermanentAddressLine1 = StudentRegistration.FatherPresentAddress;
                if (StudentRegistration.ClassNo < 11) {
                    $scope.Admission.ClassId = StudentRegistration.ClassId;
                    $scope.changeClass();
                }
                $scope.Admission.AcademicSessionId = StudentRegistration.AcademicSessionId;
                $scope.Admission.FatherName = StudentRegistration.FathersName;
                $scope.Admission.FatherOccupation = StudentRegistration.FathersOccupationDetail;
                $scope.Admission.MotherName = StudentRegistration.MothersName;
                $scope.Admission.FamilyAnnualIncome = parseInt((StudentRegistration.MothersAnnualIncome > 0 ? StudentRegistration.MothersAnnualIncome : 0) + (StudentRegistration.FathersAnnualIncome > 0 ? StudentRegistration.FathersAnnualIncome : 0));
                $scope.Admission.MotherOccupation = StudentRegistration.MothersOccupation;
                $scope.Admission.Gender = StudentRegistration.Gender;
                $scope.Admission.Email = StudentRegistration.EmailId;
                $scope.Admission.BloodGroup = StudentRegistration.BloodGroup;
                $scope.Admission.Religion = StudentRegistration.Religion;
                $scope.Admission.Nationality = StudentRegistration.Nationality;
                $scope.Admission.Category = StudentRegistration.Category;
                $scope.Admission.DateOfBirth = new Date(StudentRegistration.DOB);
                $scope.Admission.MobileNo = parseInt(StudentRegistration.MobileNo);
                $scope.Admission.PreviousSchoolName = StudentRegistration.PreviousSchoolName;
                $scope.Admission.MediumOfInstruction = StudentRegistration.PreviousSchoolMedium;
                $scope.RegistrationOnlineId = StudentRegistration.RegistrationOnlineId;
                $scope.Transport.PickupPoint = StudentRegistration.FatherPresentAddress;
                sessionStorage.removeItem('registrationOnlineDetail');
            }
        }
        $scope.checkRegistrationSession();
    }
}());
