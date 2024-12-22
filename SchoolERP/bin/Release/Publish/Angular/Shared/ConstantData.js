
(function () {
    'use strict';

    angular
        .module('app')
        .constant('ConstantData', {
            PurchaseStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ], DocumentStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ], ParentStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ], StaffWard: [
                { Key: false, Value: 'No' },
                { Key: true, Value: 'Yes' }
            ], Status: [
                //{ Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ], StatusDetail: { 1: "Active", 2: "Inactive" }
            , DesignationStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ], VehicleTypeStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ], ItemTypeStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ], MagazineStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ], RackStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ], StoreItemStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ], UnitStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            StudentStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            FathersOccupation: [
                { Key: null, Value: '--Select Fathers Occupation--' },
                { Key: 1, Value: 'BSL' },
                { Key: 2, Value: 'BCCL' },
                { Key: 3, Value: 'Gov. of Jharkhand' },
                { Key: 4, Value: 'Any Other' },
            ],
            StaffStatus: [
                { Key: null, Value: '--Select Staff Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            DepartmentStatus: [
                { Key: null, Value: '--Select Department Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            BookStatus: [
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            BookTypeStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            BookAccessionStatus: [
                { Key: 1, Value: 'Available' },
                { Key: 2, Value: 'Issued' },
                { Key: 3, Value: 'Inactive' }
            ],
            SupplierStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            ArrangementType: [
                { Key: null, Value: '--Select Arrangement Type--' },
                { Key: 1, Value: 'Office Duty' },
                { Key: 2, Value: 'On Leave' },
                { Key: 3, Value: 'Half Day Leave' }
            ], ExamType: [
                { Key: null, Value: '--Select Exam Type--' },
                { Key: 1, Value: 'Half Yearly' },
                { Key: 2, Value: 'Annual' },
                { Key: 3, Value: 'Notebook Submission' },
                { Key: 4, Value: 'Subject Enrichment Activity' },
                { Key: 5, Value: 'Periodic Test' },
                { Key: 6, Value: 'Monday Test' },
                { Key: 7, Value: 'Periodic Assessment PPT' },
                { Key: 8, Value: 'Multiple Assessment' },
                { Key: 9, Value: 'Portfolio' },
                { Key: 10, Value: 'Internal Assessment' },
                { Key: 11, Value: 'Pre-Board Exam' },
            ],
            ExaminationNames: {
                1: "PeriodicTest",
                2: "NoteBook",
                3: "SubjectEnrichment",
                4: "HalfYearly"
            },
            WhetherQualifiedForPromotion: [
                { Key: null, Value: '--Select Qualified--' },
                { Key: 1, Value: 'Yes' },
                { Key: 2, Value: 'No' }
            ],
            ExamSubjectType: [
                { Key: null, Value: '--Select Type--' },
                { Key: 1, Value: 'Grade' },
                { Key: 2, Value: 'Marks' }
            ],
            Grade: [
                { Key: 0, Value: '--Grade--' },
                { Key: 1, Value: 'A' },
                { Key: 2, Value: 'B' },
                { Key: 3, Value: 'C' },
                { Key: 4, Value: 'D' },
                { Key: 5, Value: 'E' },
            ],
            GradeType: [
                { Key: null, Value: '--Select Grade Type--' },
                { Key: 1, Value: 'Co-Scholastic Areas' },
                { Key: 2, Value: 'Co-Curricular Activity' },
                { Key: 3, Value: 'Discipline' },
                { Key: 15, Value: 'None' },
            ],
            GradeTypes: {
                1: "Co-Scholastic Areas",
                2: "Co-Curricular Activity",
                3: "Discipline",
                15: "None"
            },
            ExamTerm: [
                { Key: null, Value: '--Select Term--' },
                { Key: 1, Value: 'Term-1 (HY)' },
                { Key: 2, Value: 'Term-2 (AN)' },
                { Key: 3, Value: 'Final' }
            ],
            AnyFeeConcessionAvailed: [
                { Key: null, Value: '--Select Concession Availed--' },
                { Key: 1, Value: 'Yes' },
                { Key: 2, Value: 'No' }
            ],
            WhetherNCCCadet: [
                { Key: null, Value: '--Select NCC Cadet--' },
                { Key: 1, Value: 'Yes' },
                { Key: 2, Value: 'No' }
            ],
            ClassStatus: [
                { Key: null, Value: '--Select Class Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            SubjectOptionStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            TaskMasterStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            ClassStreamTypeStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            RoleStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            LoginStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            TeacherSubjectStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            TeacherSessionStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            TeacherStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            SubjectStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            ClassSubjectStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            SubjectType: [
                { Key: null, Value: '--Select Subject Type--' },
                { Key: 1, Value: 'Compulsory' },
                { Key: 2, Value: 'Optional' }
            ],
            SubjectType2: [
                { Key: null, Value: '--Select Subject Type--' },
                { Key: 1, Value: 'Theory' },
                { Key: 2, Value: 'Practical' }
            ],
            LeftStudentStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Left' },
                { Key: 2, Value: 'Rejoin' }
            ],
            LeftStudentType: [
                { Key: null, Value: '--Select Left Student Type--' },
                { Key: 1, Value: 'TC' },
                { Key: 2, Value: 'Not Comming' }
            ],
            EmployeeStatus: [
                { Key: null, Value: '--Select Employee Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            EmployeeTypeStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            SchoolStatus: [
                { Key: null, Value: '--Select School Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            AdmissionStatus: [
                { Key: null, Value: '--Select Admission Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            AdmissionType: [
                { Key: null, Value: '--Select Admission Type--' },
                { Key: 1, Value: 'New Admission' },
                { Key: 2, Value: 'Promoted' },
                { Key: 3, Value: 'ClassBack' }
            ],
            CityStatus: [
                { Key: null, Value: '--Select Class Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            StateStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            HouseStatus: [
                { Key: null, Value: '--Select House Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            SessionStatus: [
                { Key: null, Value: '--Select Session Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            FeeType: {
                1: "Fee",
                2: "Transport",
                3: "Fine",
                4: "Charge",
                5: "Registration",
                6: "Hostel",
                7: "PreviousDues",
                8: "ExtraCharge",
                9: "ArrearDue",
                20: "Admission",
                21: "ReAdmission",
            },
            PromotionStatus: {
                1: "Promoted",
                2: "Not Promoted"
            },
            SectionStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            StudentTypeStatus: [
                { Key: null, Value: '--Select Status--' },
                { Key: 1, Value: 'Active' },
                { Key: 2, Value: 'Inactive' }
            ],
            Gender: [
                { Key: null, Value: '--Select Gender--' },
                { Key: 1, Value: 'Male' },
                { Key: 2, Value: 'Female' }
            ],
            Nationality: [
                { Key: null, Value: '--Select Nationality--' },
                { Key: 1, Value: 'Indian' },
                { Key: 2, Value: 'Other' }
            ],
            Religion: [
                { Key: null, Value: '--Select Religion--' },
                { Key: 1, Value: 'Hindu' },
                { Key: 2, Value: 'Buddhist' },
                { Key: 3, Value: 'Sikh' },
                { Key: 4, Value: 'Jain' },
                { Key: 5, Value: 'Muslim' },
                { Key: 6, Value: 'Christian' },
                { Key: 7, Value: 'Other' }
            ],
            Category: [
                { Key: null, Value: '--Select Category--' },
                { Key: 1, Value: 'SC/ST' },
                { Key: 2, Value: 'OBC' },
                { Key: 3, Value: 'General' },
                { Key: 4, Value: 'SC' },
                { Key: 5, Value: 'ST' },
            ],
            WhetherFailedTwice: [
                { Key: null, Value: '--Select Whether Failed--' },
                { Key: 1, Value: 'Yes' },
                { Key: 2, Value: 'No' }
            ],
            BloodGroup: [
                { Key: null, Value: '--Select Blood Group--' },
                //{ Key: 1, Value: 'O' },
                { Key: 2, Value: 'O+' },
                { Key: 3, Value: 'O-' },
                //{ Key: 4, Value: 'A' },
                { Key: 5, Value: 'A+' },
                { Key: 6, Value: 'A-' },
                //{ Key: 7, Value: 'B' },
                { Key: 8, Value: 'B+' },
                { Key: 9, Value: 'B-' },
                //{ Key: 10, Value: 'AB' },
                { Key: 11, Value: 'AB-' },
                { Key: 12, Value: 'AB+' },
            ],
            PaymentMode: [
                { Key: null, Value: '--Select Payment Mode--' },
                { Key: 1, Value: 'Cash' },
                //{ Key: 2, Value: 'Card' },
                //{ Key: 3, Value: 'Bank Challan' },
                //{ Key: 4, Value: 'Demand Draft' },
                { Key: 5, Value: 'Cheque' },
                { Key: 6, Value: 'UPI' },
                //{ Key: 7, Value: 'Card Payment' },
                //{ Key: 8, Value: 'Online' },
                //{ Key: 10, Value: 'Advance' },
                //{ Key: 11, Value: 'QR Code' },
            ],
            PaymentModeAll: [
                { Key: null, Value: '--Select Payment Mode--' },
                { Key: 1, Value: 'Cash' },
                //{ Key: 2, Value: 'Card' },
                //{ Key: 3, Value: 'Bank Challan' },
                //{ Key: 4, Value: 'Demand Draft' },
                { Key: 5, Value: 'Cheque' },
                { Key: 6, Value: 'UPI' },
                //{ Key: 7, Value: 'Card Payment' },
                { Key: 8, Value: 'Online' },
                //{ Key: 10, Value: 'Advance' },
                //{ Key: 11, Value: 'QR Code' },
            ],
            PaymentModes: { 1: "Cash", 5: "Cheque", 6: "UPI", 8: "Online", 10: 'Advance', 11: 'QR Code' }
        });
})();