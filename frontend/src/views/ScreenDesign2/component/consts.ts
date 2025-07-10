export const statusMap: { [key: string]: string } = {
  "-1": "取消",
  "10": "等待OCR执行",
  "20": "OCR 执行中",
  "40": "等待Maker作业",
  "49": "返回Maker再确认",
  "50": "Maker作业中",
  "55": "Maker问询中",
  "57": "等待Checker作业",
  "58": "Checker作业中",
  "59": "Checker一时保存",
  "60": "完成",
  "95": "Error",
  "9999": "all",
  "99990": "makerComplete",
  "99991": "allWorking",
  "5": "導入完了",
};

// Cancel = -1,  //取消   2
//      WaitOCR = 10,  //等待OCR执行 5
//      OCRing = 20,   //OCR 执行中 4
//      WaitWork = 40,  //等待Maker作业 3
//      ReturnToMaker = 49 //返回Maker再确认 7
//      Working = 50,  //Maker作业中
//      Pending = 55,　//Maker问询中
//      WaitCheck=57,  //等待Checker作业
//      Checking = 58,   //Checker作业中
//      CheckRelease = 59, //Checker一时保存
//      Complete = 60,  //完成
//      Error = 95,
//      ALL=9999

export interface OptionDTO {
  value: string;
  label: string;
}
export interface Option2DTO {
  value: number;
  label: string;
}

export const statusOption: OptionDTO[] = [
  // 取消
  {
    value: "-1",
    label: "キャンセル",
  },
  // 等待OCR执行
  {
    value: "10",
    label: "OCR実行待ち",
  },
  // OCR 执行中
  {
    value: "20",
    label: "OCR実行中",
  },
  // 等待Maker作业
  {
    value: "40",
    label: "Maker作業待ち",
  },
  // 返回Maker再确认 
  {
    value: "49",
    label: "Makerに返却",
  },
  // Maker作业中
  {
    value: "50",
    label: "Maker作業中",
  },
  // Maker问询中
  {
    value: "55",
    label: "Maker確認中",
  },
  // 等待Checker作业
  {
    value: "57",
    label: "Checker作業待ち",
  },
  // Checker作业中
  {
    value: "58",
    label: "Checker作業中",
  },
  // Checker一时保存
  {
    value: "59",
    label: "一時保存",
  },
  // 完成
  {
    value: "60",
    label: "完成",
  },
  {
    value: "5",
    label: "導入完了",
  },
  {
    value: "401",
    label: "release",
  },
];

export const typeOption: OptionDTO[] = [
  {
    value: "1",
    label: "年次有給休暇",
  },
  {
    value: "2",
    label: "振替休日",
  },
  {
    value: "3",
    label: "病気休暇",
  },
  {
    value: "4",
    label: "育児休暇",
  },
  {
    value: "5",
    label: "結婚休暇",
  },
  {
    value: "6",
    label: "葬式休暇",
  },
];
export const teamNames: Record<string, string> = {
  DS: "直販",
  IS: "間販",
};
export const backends: OptionDTO[] = [
  {
    value: "DS",
    label: "直販",
  },
  {
    value: "IS",
    label: "間販",
  },
];
export const groupNames: Record<number, string> = {
  1: "東京",
  3: "大阪",
  17: "入力A",
  20: "入力B",
  21: "新SC",
};
export const periodOption: OptionDTO[] = [
  {
    value: "Day",
    label: "日",
  },
  {
    value: "Week",
    label: "周",
  },
  {
    value: "Month",
    label: "月",
  },
];
export const periodOption2: OptionDTO[] = [
  {
    value: "Daily",
    label: "日",
  },
  {
    value: "Weekly",
    label: "周",
  },
  {
    value: "Monthly",
    label: "月",
  },
];
export const businessTypeOption: Option2DTO[] = [
  // （订单处理）
  {
    value: 1,
    label: "Order processing",
  },
  // （报价管理）
  {
    value: 2,
    label: "Quotation Management",
  },
  // （销售支持）
  {
    value: 3,
    label: "Sales Support",
  },
];
export const formatStatus = (status: string) => {
  const statusItem = statusOption.find(
    (item) => `${item.value}` === `${status}`
  );
  return statusItem ? statusItem.label : "未知状态";
};
