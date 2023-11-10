using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

var pattern = new Regex(@"\s*{""([^""]+)""\s*,\s*(\w+),\s*(\w+)\s*,\s*(\w+),\s*(\w+)\s*,\s*(\w+)\s*,\s*&(\w+)}, \/\/ (\w+)", RegexOptions.Multiline);
var input = @"	{""PSET #0x%02X            ""  , 0xE40, MASK_7B , 0, 0    , 5 , &op_pset_cb}, // PSET
	{""JP   #0x%02X            ""  , 0x000, MASK_4B , 0, 0    , 5 , &op_jp_cb}, // JP
	{""JP   C #0x%02X          ""  , 0x200, MASK_4B , 0, 0    , 5 , &op_jp_c_cb}, // JP_C
	{""JP   NC #0x%02X         ""  , 0x300, MASK_4B , 0, 0    , 5 , &op_jp_nc_cb}, // JP_NC
	{""JP   Z #0x%02X          ""  , 0x600, MASK_4B , 0, 0    , 5 , &op_jp_z_cb}, // JP_Z
	{""JP   NZ #0x%02X         ""  , 0x700, MASK_4B , 0, 0    , 5 , &op_jp_nz_cb}, // JP_NZ
	{""JPBA                  ""    , 0xFE8, MASK_12B, 0, 0    , 5 , &op_jpba_cb}, // JPBA
	{""CALL #0x%02X            ""  , 0x400, MASK_4B , 0, 0    , 7 , &op_call_cb}, // CALL
	{""CALZ #0x%02X            ""  , 0x500, MASK_4B , 0, 0    , 7 , &op_calz_cb}, // CALZ
	{""RET                   ""    , 0xFDF, MASK_12B, 0, 0    , 7 , &op_ret_cb}, // RET
	{""RETS                  ""    , 0xFDE, MASK_12B, 0, 0    , 12, &op_rets_cb}, // RETS
	{""RETD #0x%02X            ""  , 0x100, MASK_4B , 0, 0    , 12, &op_retd_cb}, // RETD
	{""NOP5                  ""    , 0xFFB, MASK_12B, 0, 0    , 5 , &op_nop5_cb}, // NOP5
	{""NOP7                  ""    , 0xFFF, MASK_12B, 0, 0    , 7 , &op_nop7_cb}, // NOP7
	{""HALT                  ""    , 0xFF8, MASK_12B, 0, 0    , 5 , &op_halt_cb}, // HALT
	{""INC  X #0x%02X          ""  , 0xEE0, MASK_12B, 0, 0    , 5 , &op_inc_x_cb}, // INC_X
	{""INC  Y #0x%02X          ""  , 0xEF0, MASK_12B, 0, 0    , 5 , &op_inc_y_cb}, // INC_Y
	{""LD   X #0x%02X          ""  , 0xB00, MASK_4B , 0, 0    , 5 , &op_ld_x_cb}, // LD_X
	{""LD   Y #0x%02X          ""  , 0x800, MASK_4B , 0, 0    , 5 , &op_ld_y_cb}, // LD_Y
	{""LD   XP R(#0x%02X)      ""  , 0xE80, MASK_10B, 0, 0    , 5 , &op_ld_xp_r_cb}, // LD_XP_R
	{""LD   XH R(#0x%02X)      ""  , 0xE84, MASK_10B, 0, 0    , 5 , &op_ld_xh_r_cb}, // LD_XH_R
	{""LD   XL R(#0x%02X)      ""  , 0xE88, MASK_10B, 0, 0    , 5 , &op_ld_xl_r_cb}, // LD_XL_R
	{""LD   YP R(#0x%02X)      ""  , 0xE90, MASK_10B, 0, 0    , 5 , &op_ld_yp_r_cb}, // LD_YP_R
	{""LD   YH R(#0x%02X)      ""  , 0xE94, MASK_10B, 0, 0    , 5 , &op_ld_yh_r_cb}, // LD_YH_R
	{""LD   YL R(#0x%02X)      ""  , 0xE98, MASK_10B, 0, 0    , 5 , &op_ld_yl_r_cb}, // LD_YL_R
	{""LD   R(#0x%02X) XP      ""  , 0xEA0, MASK_10B, 0, 0    , 5 , &op_ld_r_xp_cb}, // LD_R_XP
	{""LD   R(#0x%02X) XH      ""  , 0xEA4, MASK_10B, 0, 0    , 5 , &op_ld_r_xh_cb}, // LD_R_XH
	{""LD   R(#0x%02X) XL      ""  , 0xEA8, MASK_10B, 0, 0    , 5 , &op_ld_r_xl_cb}, // LD_R_XL
	{""LD   R(#0x%02X) YP      ""  , 0xEB0, MASK_10B, 0, 0    , 5 , &op_ld_r_yp_cb}, // LD_R_YP
	{""LD   R(#0x%02X) YH      ""  , 0xEB4, MASK_10B, 0, 0    , 5 , &op_ld_r_yh_cb}, // LD_R_YH
	{""LD   R(#0x%02X) YL      ""  , 0xEB8, MASK_10B, 0, 0    , 5 , &op_ld_r_yl_cb}, // LD_R_YL
	{""ADC  XH #0x%02X         ""  , 0xA00, MASK_8B , 0, 0    , 7 , &op_adc_xh_cb}, // ADC_XH
	{""ADC  XL #0x%02X         ""  , 0xA10, MASK_8B , 0, 0    , 7 , &op_adc_xl_cb}, // ADC_XL
	{""ADC  YH #0x%02X         ""  , 0xA20, MASK_8B , 0, 0    , 7 , &op_adc_yh_cb}, // ADC_YH
	{""ADC  YL #0x%02X         ""  , 0xA30, MASK_8B , 0, 0    , 7 , &op_adc_yl_cb}, // ADC_YL
	{""CP   XH #0x%02X         ""  , 0xA40, MASK_8B , 0, 0    , 7 , &op_cp_xh_cb}, // CP_XH
	{""CP   XL #0x%02X         ""  , 0xA50, MASK_8B , 0, 0    , 7 , &op_cp_xl_cb}, // CP_XL
	{""CP   YH #0x%02X         ""  , 0xA60, MASK_8B , 0, 0    , 7 , &op_cp_yh_cb}, // CP_YH
	{""CP   YL #0x%02X         ""  , 0xA70, MASK_8B , 0, 0    , 7 , &op_cp_yl_cb}, // CP_YL
	{""LD   R(#0x%02X) #0x%02X   "", 0xE00, MASK_6B , 4, 0x030, 5 , &op_ld_r_i_cb}, // LD_R_I
	{""LD   R(#0x%02X) Q(#0x%02X)"", 0xEC0, MASK_8B , 2, 0x00C, 5 , &op_ld_r_q_cb}, // LD_R_Q
	{""LD   A M(#0x%02X)       ""  , 0xFA0, MASK_8B , 0, 0    , 5 , &op_ld_a_mn_cb}, // LD_A_MN
	{""LD   B M(#0x%02X)       ""  , 0xFB0, MASK_8B , 0, 0    , 5 , &op_ld_b_mn_cb}, // LD_B_MN
	{""LD   M(#0x%02X) A       ""  , 0xF80, MASK_8B , 0, 0    , 5 , &op_ld_mn_a_cb}, // LD_MN_A
	{""LD   M(#0x%02X) B       ""  , 0xF90, MASK_8B , 0, 0    , 5 , &op_ld_mn_b_cb}, // LD_MN_B
	{""LDPX MX #0x%02X         ""  , 0xE60, MASK_8B , 0, 0    , 5 , &op_ldpx_mx_cb}, // LDPX_MX
	{""LDPX R(#0x%02X) Q(#0x%02X)"", 0xEE0, MASK_8B , 2, 0x00C, 5 , &op_ldpx_r_cb}, // LDPX_R
	{""LDPY MY #0x%02X         ""  , 0xE70, MASK_8B , 0, 0    , 5 , &op_ldpy_my_cb}, // LDPY_MY
	{""LDPY R(#0x%02X) Q(#0x%02X)"", 0xEF0, MASK_8B , 2, 0x00C, 5 , &op_ldpy_r_cb}, // LDPY_R
	{""LBPX #0x%02X            ""  , 0x900, MASK_4B , 0, 0    , 5 , &op_lbpx_cb}, // LBPX
	{""SET  #0x%02X            ""  , 0xF40, MASK_8B , 0, 0    , 7 , &op_set_cb}, // SET
	{""RST  #0x%02X            ""  , 0xF50, MASK_8B , 0, 0    , 7 , &op_rst_cb}, // RST
	{""SCF                   ""    , 0xF41, MASK_12B, 0, 0    , 7 , &op_scf_cb}, // SCF
	{""RCF                   ""    , 0xF5E, MASK_12B, 0, 0    , 7 , &op_rcf_cb}, // RCF
	{""SZF                   ""    , 0xF42, MASK_12B, 0, 0    , 7 , &op_szf_cb}, // SZF
	{""RZF                   ""    , 0xF5D, MASK_12B, 0, 0    , 7 , &op_rzf_cb}, // RZF
	{""SDF                   ""    , 0xF44, MASK_12B, 0, 0    , 7 , &op_sdf_cb}, // SDF
	{""RDF                   ""    , 0xF5B, MASK_12B, 0, 0    , 7 , &op_rdf_cb}, // RDF
	{""EI                    ""    , 0xF48, MASK_12B, 0, 0    , 7 , &op_ei_cb}, // EI
	{""DI                    ""    , 0xF57, MASK_12B, 0, 0    , 7 , &op_di_cb}, // DI
	{""INC  SP               ""    , 0xFDB, MASK_12B, 0, 0    , 5 , &op_inc_sp_cb}, // INC_SP
	{""DEC  SP               ""    , 0xFCB, MASK_12B, 0, 0    , 5 , &op_dec_sp_cb}, // DEC_SP
	{""PUSH R(#0x%02X)         ""  , 0xFC0, MASK_10B, 0, 0    , 5 , &op_push_r_cb}, // PUSH_R
	{""PUSH XP               ""    , 0xFC4, MASK_12B, 0, 0    , 5 , &op_push_xp_cb}, // PUSH_XP
	{""PUSH XH               ""    , 0xFC5, MASK_12B, 0, 0    , 5 , &op_push_xh_cb}, // PUSH_XH
	{""PUSH XL               ""    , 0xFC6, MASK_12B, 0, 0    , 5 , &op_push_xl_cb}, // PUSH_XL
	{""PUSH YP               ""    , 0xFC7, MASK_12B, 0, 0    , 5 , &op_push_yp_cb}, // PUSH_YP
	{""PUSH YH               ""    , 0xFC8, MASK_12B, 0, 0    , 5 , &op_push_yh_cb}, // PUSH_YH
	{""PUSH YL               ""    , 0xFC9, MASK_12B, 0, 0    , 5 , &op_push_yl_cb}, // PUSH_YL
	{""PUSH F                ""    , 0xFCA, MASK_12B, 0, 0    , 5 , &op_push_f_cb}, // PUSH_F
	{""POP  R(#0x%02X)         ""  , 0xFD0, MASK_10B, 0, 0    , 5 , &op_pop_r_cb}, // POP_R
	{""POP  XP               ""    , 0xFD4, MASK_12B, 0, 0    , 5 , &op_pop_xp_cb}, // POP_XP
	{""POP  XH               ""    , 0xFD5, MASK_12B, 0, 0    , 5 , &op_pop_xh_cb}, // POP_XH
	{""POP  XL               ""    , 0xFD6, MASK_12B, 0, 0    , 5 , &op_pop_xl_cb}, // POP_XL
	{""POP  YP               ""    , 0xFD7, MASK_12B, 0, 0    , 5 , &op_pop_yp_cb}, // POP_YP
	{""POP  YH               ""    , 0xFD8, MASK_12B, 0, 0    , 5 , &op_pop_yh_cb}, // POP_YH
	{""POP  YL               ""    , 0xFD9, MASK_12B, 0, 0    , 5 , &op_pop_yl_cb}, // POP_YL
	{""POP  F                ""    , 0xFDA, MASK_12B, 0, 0    , 5 , &op_pop_f_cb}, // POP_F
	{""LD   SPH R(#0x%02X)     ""  , 0xFE0, MASK_10B, 0, 0    , 5 , &op_ld_sph_r_cb}, // LD_SPH_R
	{""LD   SPL R(#0x%02X)     ""  , 0xFF0, MASK_10B, 0, 0    , 5 , &op_ld_spl_r_cb}, // LD_SPL_R
	{""LD   R(#0x%02X) SPH     ""  , 0xFE4, MASK_10B, 0, 0    , 5 , &op_ld_r_sph_cb}, // LD_R_SPH
	{""LD   R(#0x%02X) SPL     ""  , 0xFF4, MASK_10B, 0, 0    , 5 , &op_ld_r_spl_cb}, // LD_R_SPL
	{""ADD  R(#0x%02X) #0x%02X   "", 0xC00, MASK_6B , 4, 0x030, 7 , &op_add_r_i_cb}, // ADD_R_I
	{""ADD  R(#0x%02X) Q(#0x%02X)"", 0xA80, MASK_8B , 2, 0x00C, 7 , &op_add_r_q_cb}, // ADD_R_Q
	{""ADC  R(#0x%02X) #0x%02X   "", 0xC40, MASK_6B , 4, 0x030, 7 , &op_adc_r_i_cb}, // ADC_R_I
	{""ADC  R(#0x%02X) Q(#0x%02X)"", 0xA90, MASK_8B , 2, 0x00C, 7 , &op_adc_r_q_cb}, // ADC_R_Q
	{""SUB  R(#0x%02X) Q(#0x%02X)"", 0xAA0, MASK_8B , 2, 0x00C, 7 , &op_sub_cb}, // SUB
	{""SBC  R(#0x%02X) #0x%02X   "", 0xB40, MASK_6B , 4, 0x030, 7 , &op_sbc_r_i_cb}, // SBC_R_I
	{""SBC  R(#0x%02X) Q(#0x%02X)"", 0xAB0, MASK_8B , 2, 0x00C, 7 , &op_sbc_r_q_cb}, // SBC_R_Q
	{""AND  R(#0x%02X) #0x%02X   "", 0xC80, MASK_6B , 4, 0x030, 7 , &op_and_r_i_cb}, // AND_R_I
	{""AND  R(#0x%02X) Q(#0x%02X)"", 0xAC0, MASK_8B , 2, 0x00C, 7 , &op_and_r_q_cb}, // AND_R_Q
	{""OR   R(#0x%02X) #0x%02X   "", 0xCC0, MASK_6B , 4, 0x030, 7 , &op_or_r_i_cb}, // OR_R_I
	{""OR   R(#0x%02X) Q(#0x%02X)"", 0xAD0, MASK_8B , 2, 0x00C, 7 , &op_or_r_q_cb}, // OR_R_Q
	{""XOR  R(#0x%02X) #0x%02X   "", 0xD00, MASK_6B , 4, 0x030, 7 , &op_xor_r_i_cb}, // XOR_R_I
	{""XOR  R(#0x%02X) Q(#0x%02X)"", 0xAE0, MASK_8B , 2, 0x00C, 7 , &op_xor_r_q_cb}, // XOR_R_Q
	{""CP   R(#0x%02X) #0x%02X   "", 0xDC0, MASK_6B , 4, 0x030, 7 , &op_cp_r_i_cb}, // CP_R_I
	{""CP   R(#0x%02X) Q(#0x%02X)"", 0xF00, MASK_8B , 2, 0x00C, 7 , &op_cp_r_q_cb}, // CP_R_Q
	{""FAN  R(#0x%02X) #0x%02X   "", 0xD80, MASK_6B , 4, 0x030, 7 , &op_fan_r_i_cb}, // FAN_R_I
	{""FAN  R(#0x%02X) Q(#0x%02X)"", 0xF10, MASK_8B , 2, 0x00C, 7 , &op_fan_r_q_cb}, // FAN_R_Q
	{""RLC  R(#0x%02X)         ""  , 0xAF0, MASK_8B , 0, 0    , 7 , &op_rlc_cb}, // RLC
	{""RRC  R(#0x%02X)         ""  , 0xE8C, MASK_10B, 0, 0    , 5 , &op_rrc_cb}, // RRC
	{""INC  M(#0x%02X)         ""  , 0xF60, MASK_8B , 0, 0    , 7 , &op_inc_mn_cb}, // INC_MN
	{""DEC  M(#0x%02X)         ""  , 0xF70, MASK_8B , 0, 0    , 7 , &op_dec_mn_cb}, // DEC_MN
	{""ACPX R(#0x%02X)         ""  , 0xF28, MASK_10B, 0, 0    , 7 , &op_acpx_cb}, // ACPX
	{""ACPY R(#0x%02X)         ""  , 0xF2C, MASK_10B, 0, 0    , 7 , &op_acpy_cb}, // ACPY
	{""SCPX R(#0x%02X)         ""  , 0xF38, MASK_10B, 0, 0    , 7 , &op_scpx_cb}, // SCPX
	{""SCPY R(#0x%02X)         ""  , 0xF3C, MASK_10B, 0, 0    , 7 , &op_scpy_cb}, // SCPY
	{""NOT  R(#0x%02X)         ""  , 0xD0F, 0xFCF   , 4, 0    , 7 , &op_not_cb}, // NOT";

var instructions = new List<Instruction>();
foreach (Match match in pattern.Matches(input))
{
    var log = match.Groups[1].Value.Trim();
	var code = ParseInt(match.Groups[2].Value);
	var mask = ParseMask(match.Groups[3].Value.Trim());
    var shiftArg0 = ParseInt(match.Groups[4].Value);
    var maskArg0 = ParseInt(match.Groups[5].Value);
    var cycles = ParseInt(match.Groups[6].Value);
	var method = match.Groups[7].Value;
    var name = match.Groups[8].Value;

	instructions.Add(new Instruction(log, code, mask, shiftArg0, maskArg0, cycles, method, name));
}

var lineCount = input.Count(c => c == '{');
if (instructions.Count != lineCount) throw new Exception("missing instructions");

var conflicts = new Dictionary<string, List<Instruction>>();
for (var i = 1; i < instructions.Count; i++)
{
	var instruction = instructions[i];
    for (var j = 0; j < i; j++)
    {
        var compare = instructions[j];
		// does compare match a subset of instruction?
		// (op & mask) == code
        if ((instruction.code & compare.mask) == compare.code && instruction.mask > compare.mask)
        {
            Console.WriteLine($"{instruction.name} hides some of {compare.name}");
            
            if (!conflicts.TryGetValue(instruction.name, out var conflictInstructions))
            {
				conflictInstructions = new List<Instruction>();
				conflicts.Add(instruction.name, conflictInstructions);
            }

			conflictInstructions.Add(compare);
        }
    }
}

var writer = new IndentTextWriter(new StringWriter(), "\t");
writer.WriteLine("u8_t is_pset = 0;");

var masked = instructions.OrderByDescending(i => i.mask).GroupBy(i => i.mask).ToList();
foreach (var group in masked)
{
	var mask = group.Key;
	writer.WriteLine($"switch (op & {MaskToString(mask)})");
	writer.OpenBracket();

    foreach (var instruction in group)
    {
		writer.WriteLine($"case 0x{instruction.code:X3}:");
        writer.Indent++;

        var hasConflicts = false;
        if (conflicts.TryGetValue(instruction.name, out var conflictInstructions))
        {
            hasConflicts = true;

            for (var i = 0; i < conflictInstructions.Count; i++)
            {
				var conflict = conflictInstructions[i];
				writer.WriteLine($"if ((op & {MaskToString(conflict.mask)}) == 0x{conflict.code:X3})");
				writer.OpenBracket();
				WriteInstructionHandler(conflict);
				writer.CloseBracket();

                if (i < conflictInstructions.Count - 1)
                {
					writer.Write("else ");
                }
            }

			writer.WriteLine("else");
			writer.OpenBracket();
        }

		WriteInstructionHandler(instruction);

        if (hasConflicts)
        {
			writer.CloseBracket();
        }

        writer.WriteLine("break;");
		writer.Indent--;
    }

	writer.WriteLine("default:");
	writer.OpenBracket();
}

writer.WriteLine("//g_hal->log(LOG_ERROR, \"Unknown op-code 0x%X (pc = 0x%04X)\\n\", op, cpu->pc);");

for (var i = 0; i < masked.Count; i++)
{
	writer.WriteLine("break;");
	writer.CloseBracket();
	writer.CloseBracket();
}

void WriteInstructionHandler(Instruction instruction)
{
    writer.WriteLine($"// {instruction.name} :: {instruction.log}");
    if (instruction.maskArg0 != 0)
    {
        // two args
        writer.WriteLine($"{instruction.method}(cpu, (op & 0x{instruction.maskArg0:X3}) >> {instruction.shiftArg0}, op & ~(0x{instruction.mask:X3} | 0x{instruction.maskArg0:X3}));");
    }
    else
    {
        // one arg
        writer.WriteLine($"{instruction.method}(cpu, (op & ~0x{instruction.mask:X3}) >> {instruction.shiftArg0}, 0);");
    }
    writer.WriteLine($"cpu->previous_cycles = {instruction.cycles};");
    if (instruction.name == "PSET")
    {
        writer.WriteLine("is_pset = 1;");
    }

}

//Console.WriteLine(writer.ToString());
File.WriteAllText("dispatch.txt", writer.ToString());

static int ParseInt(string input) => input.StartsWith("0x") ? int.Parse(input.Substring(2), NumberStyles.HexNumber) : int.Parse(input);

static int ParseMask(string input)
{
    switch (input)
    {
		case "MASK_12B": return 0xFFF;
		case "MASK_10B": return 0xFFC;
		case "MASK_8B": return 0xFF0;
		case "MASK_7B": return 0xFE0;
		case "MASK_6B": return 0xFC0;
		case "MASK_4B": return 0xF00;
		default: return ParseInt(input);
    }
}

static string MaskToString(int mask)
{
    switch (mask)
    {
		case 0xFFF: return "MASK_12B";
		case 0xFFC: return "MASK_10B";
		case 0xFF0: return "MASK_8B";
		case 0xFE0: return "MASK_7B";
		case 0xFC0: return "MASK_6B";
		case 0xF00: return "MASK_4B";
		default: return $"0x{mask:X3}";
    }
}

record Instruction(string log, int code, int mask, int shiftArg0, int maskArg0, int cycles, string method, string name);

public class IndentTextWriter : TextWriter
{
    private readonly TextWriter _writer;
    private readonly string _indentStr;
    private bool _shouldIndent;

    public int Indent { get; set; }

    public IndentTextWriter(TextWriter writer, string indentStr = "    ")
    {
        _writer = writer;
        _indentStr = indentStr;
        _shouldIndent = false;
    }

    public override Encoding Encoding => Encoding.Unicode;

    public override IFormatProvider FormatProvider => CultureInfo.InvariantCulture;

    public void OpenBracket(string before = null)
    {
        if (before != null)
        {
            Write(before);
        }

        WriteLine("{");
        Indent++;
    }

    public void CloseBracket(string after = null)
    {
        Indent--;

        if (after != null)
        {
            Write("}");
            WriteLine(after);
        }
        else
        {
            WriteLine("}");
        }
    }

    public override void Write(char value)
    {
        if (_shouldIndent)
        {
            _shouldIndent = false; // shouldIndent must be cleared first
            WriteIndent();
        }

        _writer.Write(value);
    }

    public override void WriteLine()
    {
        base.WriteLine();

        _shouldIndent = true; // defer indenting until the next Write
    }

    public override void WriteLine(string value)
    {
        Write(value);
        WriteLine();
    }

    private void WriteIndent()
    {
        for (var i = 0; i < Indent; i++)
        {
            Write(_indentStr);
        }
    }

    public override string ToString()
    {
        return _writer.ToString();
    }
}
