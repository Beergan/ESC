window.escFormulaGuide = {
    startCreateFormulaGuide: function () {
        if (!window.driver || !window.driver.js || !window.driver.js.driver) {
            console.warn("Driver.js is not loaded.");
            return;
        }

        const driverObj = window.driver.js.driver({
            showProgress: true,
            allowClose: true,
            animate: true,
            smoothScroll: true,
            nextBtnText: "다음 / Next",
            prevBtnText: "이전 / Back",
            doneBtnText: "완료 / Done",
            closeBtnText: "닫기 / Close",
            steps: [
                {
                    element: '[data-guide="formula-status"]',
                    popover: {
                        title: "현재 상태 확인 / Check Status",
                        description: "여기에서 이 계산식이 현재 적용 중인지, 저장만 된 상태인지, 비활성 상태인지 확인할 수 있습니다.",
                        side: "bottom",
                        align: "start"
                    }
                },
                {
                    element: '[data-guide="tab-basic"]',
                    popover: {
                        title: "기본 정보 / Basic Information",
                        description: "먼저 계산식 코드, 이름, 버전 정보를 입력합니다. 코드는 내부 구분용이고, 이름은 관리자가 보기 쉽게 작성하면 됩니다.",
                        side: "bottom",
                        align: "start"
                    }
                },
                {
                    element: '[data-guide="formula-code"]',
                    popover: {
                        title: "계산식 코드 / Formula Code",
                        description: "예: DEFAULT_ESC, ESC_2026_V1 처럼 짧고 구분하기 쉬운 코드를 입력하세요.",
                        side: "bottom",
                        align: "start"
                    }
                },
                {
                    element: '[data-guide="formula-name"]',
                    popover: {
                        title: "계산식 이름 / Formula Name",
                        description: "관리자가 이해하기 쉬운 이름을 입력하세요. 예: 기본 ESC 계산식, 2026년 변경 계산식.",
                        side: "bottom",
                        align: "start"
                    }
                },
                {
                    element: '[data-guide="formula-status-options"]',
                    popover: {
                        title: "적용 상태 / Apply Status",
                        description: "Active는 사용 가능 상태입니다. Current를 체크하면 모든 사용자 계산에 이 계산식이 자동 적용됩니다.",
                        side: "bottom",
                        align: "start"
                    }
                },
                {
                    element: '[data-guide="tab-formula"]',
                    popover: {
                        title: "계산식 입력 / Formula Input",
                        description: "여기에서 실제 계산 공식을 관리합니다. 잘 모르면 기본값을 유지하고, 필요한 경우만 수정하세요.",
                        side: "bottom",
                        align: "start",
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
                        title: "공식 작성 안내 / Formula Guide",
                        description: "공식은 아래 변수명을 사용합니다. 예: Amount / TotalCost, CompareIndex / BaseIndex.",
                        side: "bottom",
                        align: "start"
                    }
                },
                {
                    element: '[data-guide="weight-formula"]',
                    popover: {
                        title: "가중치 계산식 / Weight Formula",
                        description: "각 비목 금액이 전체 금액에서 차지하는 비율입니다. 기본값은 Amount / TotalCost 입니다.",
                        side: "bottom",
                        align: "start"
                    }
                },
                {
                    element: '[data-guide="adjustment-rate-formula"]',
                    popover: {
                        title: "등락율 계산식 / Adjustment Rate",
                        description: "최종 변동률 K를 계산합니다. 기본값은 (CompositeCoefficient - 1) * 100 입니다.",
                        side: "bottom",
                        align: "start"
                    }
                },
                {
                    element: '[data-guide="tab-condition"]',
                    popover: {
                        title: "조건 설정 / Conditions",
                        description: "기준 등락율, 기준 경과일수, 반올림 방식, 선금공제 사용 여부를 설정합니다.",
                        side: "bottom",
                        align: "start",
                        onNextClick: function () {
                            const btn = document.querySelector('[data-guide="tab-condition"]');
                            if (btn) btn.click();
                            setTimeout(() => driverObj.moveNext(), 250);
                        }
                    }
                },
                {
                    element: '[data-guide="condition-area"]',
                    popover: {
                        title: "적용 조건 / Apply Conditions",
                        description: "기본 조건은 등락율 3% 이상, 경과일수 90일 이상입니다.",
                        side: "bottom",
                        align: "start"
                    }
                },
                {
                    element: '[data-guide="tab-test"]',
                    popover: {
                        title: "테스트 계산 / Test Calculation",
                        description: "저장하기 전에 샘플 값으로 계산 결과가 정상인지 확인합니다.",
                        side: "bottom",
                        align: "start",
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
                        title: "테스트 실행 / Run Test",
                        description: "이 버튼을 눌러 현재 입력한 공식으로 계산이 되는지 확인하세요.",
                        side: "bottom",
                        align: "start"
                    }
                },
                {
                    element: '[data-guide="save-button"]',
                    popover: {
                        title: "저장 / Save",
                        description: "테스트가 정상이라면 저장합니다. Current로 설정하면 사용자 계산에 바로 적용됩니다.",
                        side: "top",
                        align: "end"
                    }
                }
            ]
        });

        driverObj.drive();
    },

    startQuickGuide: function () {
        if (!window.driver || !window.driver.js || !window.driver.js.driver) {
            console.warn("Driver.js is not loaded.");
            return;
        }

        const driverObj = window.driver.js.driver({
            showProgress: true,
            allowClose: true,
            animate: true,
            smoothScroll: true,
            nextBtnText: "다음 / Next",
            prevBtnText: "이전 / Back",
            doneBtnText: "완료 / Done",
            closeBtnText: "닫기 / Close",
            steps: [
                {
                    element: '[data-guide="create-button"]',
                    popover: {
                        title: "새 계산식 만들기 / Create New Formula",
                        description: "새로운 계산식을 만들 때 이 버튼을 누릅니다. 기존 공식을 복사해서 새 버전으로 관리하는 방식이 안전합니다.",
                        side: "bottom",
                        align: "end"
                    }
                },
                {
                    element: '[data-guide="formula-list"]',
                    popover: {
                        title: "계산식 목록 / Formula List",
                        description: "현재 등록된 계산식 목록입니다. Current 표시가 있는 계산식이 사용자 계산에 적용됩니다.",
                        side: "right",
                        align: "start"
                    }
                },
                {
                    element: '[data-guide="save-button"]',
                    popover: {
                        title: "저장 / Save",
                        description: "수정 후 저장해야 변경 내용이 반영됩니다.",
                        side: "top",
                        align: "end"
                    }
                }
            ]
        });

        driverObj.drive();
    }
};