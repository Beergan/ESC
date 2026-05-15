window.escFormulaGuide = {
    // Helper to pick text based on language
    t: function (lang, kr, en) {
        return lang === 'ko' ? kr : en;
    },

    startCreateFormulaGuide: function (lang) {
        if (!window.driver || !window.driver.js || !window.driver.js.driver) {
            console.warn("Driver.js is not loaded.");
            return;
        }

        const self = this;
        const driverObj = window.driver.js.driver({
            showProgress: true,
            allowClose: true,
            animate: true,
            smoothScroll: true,
            nextBtnText: self.t(lang, "다음", "Next"),
            prevBtnText: self.t(lang, "이전", "Back"),
            doneBtnText: self.t(lang, "완료", "Done"),
            closeBtnText: self.t(lang, "닫기", "Close"),
            popoverClass: 'driverjs-theme', // Use custom theme
            steps: [
                {
                    element: '[data-guide="formula-status"]',
                    popover: {
                        title: self.t(lang, "상태 확인", "Check Status"),
                        description: self.t(lang, "여기에서 이 계산식이 현재 적용 중인지, 저장만 된 상태인지 확인할 수 있습니다.", "Check if this formula is currently applied, saved, or inactive."),
                        side: "bottom", align: "start"
                    }
                },
                {
                    element: '[data-guide="tab-basic"]',
                    popover: {
                        title: self.t(lang, "기본 정보", "Basic Info"),
                        description: self.t(lang, "계산식 코드, 이름, 버전 정보를 입력하는 탭입니다.", "Tab for entering formula code, name, and version info."),
                        side: "bottom", align: "start"
                    }
                },
                {
                    element: '[data-guide="formula-code"]',
                    popover: {
                        title: self.t(lang, "계산식 코드", "Formula Code"),
                        description: self.t(lang, "예: DEFAULT_ESC, ESC_2026_V1 처럼 구분하기 쉬운 코드를 입력하세요.", "Enter a unique code like DEFAULT_ESC or ESC_2026_V1."),
                        side: "bottom", align: "start"
                    }
                },
                {
                    element: '[data-guide="formula-name"]',
                    popover: {
                        title: self.t(lang, "계산식 이름", "Formula Name"),
                        description: self.t(lang, "관리자가 이해하기 쉬운 이름을 입력하세요.", "Enter a name that is easy for admins to understand."),
                        side: "bottom", align: "start"
                    }
                },
                {
                    element: '[data-guide="formula-status-options"]',
                    popover: {
                        title: self.t(lang, "적용 상태", "Apply Status"),
                        description: self.t(lang, "Active는 사용 가능, Current를 체크하면 즉시 시스템에 적용됩니다.", "Active enables it, Current applies it to the system immediately."),
                        side: "bottom", align: "start"
                    }
                },
                {
                    element: '[data-guide="tab-formula"]',
                    popover: {
                        title: self.t(lang, "계산식 입력", "Formula Configuration"),
                        description: self.t(lang, "실제 계산 공식을 관리하는 탭입니다.", "Tab for managing the actual calculation formulas."),
                        side: "bottom", align: "start",
                        onNextClick: function () {
                            const btn = document.querySelector('[data-guide="tab-formula"]');
                            if (btn) btn.click();
                            setTimeout(() => driverObj.moveNext(), 250);
                        }
                    }
                },
                {
                    element: '[data-guide="formula-help"]',
                    popover: {
                        title: self.t(lang, "공식 작성 안내", "Formula Guide"),
                        description: self.t(lang, "공식 작성 시 사용할 수 있는 변수와 연산자 안내입니다.", "Guide for variables and operators available for formulas."),
                        side: "bottom", align: "start"
                    }
                },
                {
                    element: '[data-guide="weight-formula"]',
                    popover: {
                        title: self.t(lang, "가중치 계산식", "Weight Formula"),
                        description: self.t(lang, "비목별 가중치를 계산하는 공식입니다.", "Formula to calculate weight for each cost item."),
                        side: "bottom", align: "start"
                    }
                },
                {
                    element: '[data-guide="adjustment-rate-formula"]',
                    popover: {
                        title: self.t(lang, "등락율 계산식", "Adjustment Rate"),
                        description: self.t(lang, "최종 변동률 K를 계산하는 공식입니다.", "Formula to calculate the final adjustment rate K."),
                        side: "bottom", align: "start"
                    }
                },
                {
                    element: '[data-guide="tab-condition"]',
                    popover: {
                        title: self.t(lang, "조건 설정", "Condition Settings"),
                        description: this.t(lang, "기준 등락율, 반올림 방식 등을 설정합니다.", "Set threshold rates, rounding methods, etc."),
                        side: "bottom", align: "start",
                        onNextClick: function () {
                            const btn = document.querySelector('[data-guide="tab-option"]');
                            if (btn) btn.click();
                            setTimeout(() => driverObj.moveNext(), 250);
                        }
                    }
                },
                {
                    element: '[data-guide="condition-area"]',
                    popover: {
                        title: self.t(lang, "적용 조건", "Apply Conditions"),
                        description: self.t(lang, "기본 조건은 3% 이상, 90일 이상입니다.", "Default conditions: 3% or more, 90 days or more."),
                        side: "bottom", align: "start"
                    }
                },
                {
                    element: '[data-guide="tab-test"]',
                    popover: {
                        title: self.t(lang, "테스트 계산", "Test Calculation"),
                        description: self.t(lang, "저장 전 샘플 값으로 결과를 확인하는 탭입니다.", "Tab to verify results with sample values before saving."),
                        side: "bottom", align: "start",
                        onNextClick: function () {
                            const btn = document.querySelector('[data-guide="tab-test"]');
                            if (btn) btn.click();
                            setTimeout(() => driverObj.moveNext(), 250);
                        }
                    }
                },
                {
                    element: '[data-guide="test-button"]',
                    popover: {
                        title: self.t(lang, "테스트 실행", "Run Test"),
                        description: self.t(lang, "이 버튼을 눌러 계산 로직을 검증하세요.", "Click this button to validate the calculation logic."),
                        side: "bottom", align: "start"
                    }
                },
                {
                    element: '[data-guide="save-button"]',
                    popover: {
                        title: self.t(lang, "저장", "Save"),
                        description: self.t(lang, "모든 설정이 완료되면 저장합니다.", "Save all settings when finished."),
                        side: "top", align: "end"
                    }
                }
            ]
        });

        driverObj.drive();
    },

    startQuickGuide: function (lang) {
        if (!window.driver || !window.driver.js || !window.driver.js.driver) {
            console.warn("Driver.js is not loaded.");
            return;
        }

        const self = this;
        const driverObj = window.driver.js.driver({
            showProgress: true,
            allowClose: true,
            animate: true,
            smoothScroll: true,
            nextBtnText: self.t(lang, "다음", "Next"),
            prevBtnText: self.t(lang, "이전", "Back"),
            doneBtnText: self.t(lang, "완료", "Done"),
            closeBtnText: self.t(lang, "닫기", "Close"),
            popoverClass: 'driverjs-theme',
            steps: [
                {
                    element: '[data-guide="create-button"]',
                    popover: {
                        title: self.t(lang, "새 계산식 만들기", "Create New Formula"),
                        description: self.t(lang, "새로운 계산식을 등록하거나 기존 것을 복사할 수 있습니다.", "Register a new formula or duplicate an existing one."),
                        side: "bottom", align: "end"
                    }
                },
                {
                    element: '[data-guide="formula-list"]',
                    popover: {
                        title: self.t(lang, "계산식 목록", "Formula List"),
                        description: self.t(lang, "등록된 모든 계산식 히스토리를 확인합니다.", "Check the history of all registered formulas."),
                        side: "right", align: "start"
                    }
                },
                {
                    element: '[data-guide="save-button"]',
                    popover: {
                        title: self.t(lang, "저장", "Save"),
                        description: self.t(lang, "변경 내용을 최종 반영합니다.", "Apply your changes to the system."),
                        side: "top", align: "end"
                    }
                }
            ]
        });

        driverObj.drive();
    }
};