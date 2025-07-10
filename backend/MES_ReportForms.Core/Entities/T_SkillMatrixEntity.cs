
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_ReportForms.Core.Entities
{
    [Table("T_SkillMatrix")]
    public class T_SkillMatrixEntity
    {
        /// <summary>
        /// 用户的全局唯一标识符，设置为主键
        /// </summary>
        [Key]
        public string GUID { get; set; }

        /// <summary>
        /// 团队
        /// </summary>
        public string Team { get; set; }

        /// <summary>
        /// 分组 1
        /// </summary>
        public string Group1 { get; set; }

        /// <summary>
        /// 分组 2
        /// </summary>
        public string Group2 { get; set; }

        /// <summary>
        /// 订单处理技能 - 东京
        /// </summary>
        public string TokyoSkill { get; set; }

        /// <summary>
        /// 订单处理技能 - 大阪
        /// </summary>
        public string OsakaSkill { get; set; }

        /// <summary>
        /// 订单处理技能 - Wave10
        /// </summary>
        public string Wave10Skill { get; set; }

        /// <summary>
        /// 订单处理技能 - 入力 A
        /// </summary>
        public string InputASkill { get; set; }

        /// <summary>
        /// 订单处理技能 - 入力 B
        /// </summary>
        public string InputBSkill { get; set; }

        /// <summary>
        /// 订单处理技能 - 新 SC
        /// </summary>
        public string NewSCSkill { get; set; }

        /// <summary>
        /// 订单处理技能 - E - FAX 送信
        /// </summary>
        public string EFAXSkill { get; set; }

        /// <summary>
        /// 报价管理技能 - 物販見積作成
        /// </summary>
        public string ProductQuoteSkill { get; set; }

        /// <summary>
        /// 报价管理技能 - GPO 二次見積
        /// </summary>
        public string GPOSecondQuoteSkill { get; set; }

        /// <summary>
        /// 报价管理技能 - 契約見積作成
        /// </summary>
        public string ContractQuoteSkill { get; set; }

        /// <summary>
        /// 报价管理技能 - マスター登録申請
        /// </summary>
        public string MasterRegistrationSkill { get; set; }

        /// <summary>
        /// 报价管理技能 - 証明書 MSA 入金業務
        /// </summary>
        public string CertificateSkill { get; set; }

        /// <summary>
        /// 报价管理技能 - MSA 入金業務
        /// </summary>
        public string MSAPaymentSkill { get; set; }

        /// <summary>
        /// 报价管理技能 - 後値引き
        /// </summary>
        public string PostDiscountSkill { get; set; }

        /// <summary>
        /// 销售支持技能 - サンプル手配
        /// </summary>
        public string SampleArrangementSkill { get; set; }

        /// <summary>
        /// 销售支持技能 - 短期貸出（直販＆間販）
        /// </summary>
        public string ShortTermLoanSkill { get; set; }

        /// <summary>
        /// 销售支持技能 - 在庫関連＆販促品
        /// </summary>
        public string InventoryPromotionSkill { get; set; }

        /// <summary>
        /// 销售支持技能 - ローナー備品
        /// </summary>
        public string LoanerEquipmentSkill { get; set; }

        /// <summary>
        /// 销售支持技能 - 備品手配関連(ローナー以外)
        /// </summary>
        public string EquipmentArrangementSkill { get; set; }

        /// <summary>
        /// 销售支持技能 - 間販案件
        /// </summary>
        public string IndirectSalesCaseSkill { get; set; }

        /// <summary>
        /// 销售支持技能 - 新 SC 案件
        /// </summary>
        public string NewSCCaseSkill { get; set; }

        /// <summary>
        /// 销售支持技能 - 直販案件
        /// </summary>
        public string DirectSalesCaseSkill { get; set; }

        /// <summary>
        /// 语言技能 - 日语证书
        /// </summary>
        public string JapaneseCertificate { get; set; }

        /// <summary>
        /// 语言技能 - 日语实际能力
        /// </summary>
        public string JapaneseAbility { get; set; }

        /// <summary>
        /// 语言技能 - 留学经验
        /// </summary>
        public string StudyAbroadExperience { get; set; }

        /// <summary>
        /// 语言技能 - 英语等级
        /// </summary>
        public string EnglishLevel { get; set; }

        /// <summary>
        /// 语言技能 - Call 对应经验
        /// </summary>
        public string CallHandlingExperience { get; set; }

        /// <summary>
        /// 其他技能 - KT 经验
        /// </summary>
        public string KTExperience { get; set; }

        /// <summary>
        /// 其他技能 - Domain 经验①(2 年以上)
        /// </summary>
        public string DomainExperience1 { get; set; }

        /// <summary>
        /// 其他技能 - Domain 经验②(2 年以上)
        /// </summary>
        public string DomainExperience2 { get; set; }

        /// <summary>
        /// 其他技能 - Excel 技能
        /// </summary>
        public string ExcelSkill { get; set; }

        /// <summary>
        /// 其他技能 - その他資格
        /// </summary>
        public string OtherQualifications { get; set; }

        /// <summary>
        /// 其他技能 - その他スキル
        /// </summary>
        public string OtherSkills { get; set; }
    }

}
