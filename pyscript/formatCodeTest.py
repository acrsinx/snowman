import unittest
import formatCode

class TestFormatCode(unittest.TestCase):
    def test_is_operator(self):
        self.assertTrue(formatCode.is_operator("+"))
        self.assertTrue(formatCode.is_operator("-"))
        self.assertTrue(formatCode.is_operator("*"))
        self.assertTrue(formatCode.is_operator("/"))
        self.assertFalse(formatCode.is_operator("a"))
        self.assertFalse(formatCode.is_operator("1"))
        self.assertFalse(formatCode.is_operator(" "))
        self.assertFalse(formatCode.is_operator("()"))

    def test_merge_words(self):
        input_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("a", formatCode.NoteType.NORMAL, False),
            ("b", formatCode.NoteType.NORMAL, False),
            ("c", formatCode.NoteType.NORMAL, False)
        ]
        output_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("abc", formatCode.NoteType.NORMAL, False)
        ]
        formatCode.merge_words(input_code, 0, 2)
        self.assertEqual(input_code, output_code)
        self.assertEqual(len(input_code), 1)

    def test_find_and_combine_ternary_operator(self):
        input_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("a", formatCode.NoteType.NORMAL, False),
            ("?", formatCode.NoteType.NORMAL, False),
            ("b", formatCode.NoteType.NORMAL, False),
            (":", formatCode.NoteType.NORMAL, False),
            ("c", formatCode.NoteType.NORMAL, False),
            (";", formatCode.NoteType.NORMAL, False),
            ("d", formatCode.NoteType.NORMAL, False),
            (":", formatCode.NoteType.NORMAL, False)
        ]
        output_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("a?b:c", formatCode.NoteType.NORMAL, False),
            (";", formatCode.NoteType.NORMAL, False),
            ("d", formatCode.NoteType.NORMAL, False),
            (":", formatCode.NoteType.NORMAL, False)
        ]
        formatCode.find_and_combine_ternary_operator(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_colon(self):
        input_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("class", formatCode.NoteType.NORMAL, False),
            ("a", formatCode.NoteType.NORMAL, False),
            (":", formatCode.NoteType.NORMAL, False),
            ("{", formatCode.NoteType.NORMAL, False),
            ("}", formatCode.NoteType.NORMAL, False)
        ]
        output_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("class", formatCode.NoteType.NORMAL, False),
            ("a:", formatCode.NoteType.NORMAL, False),
            ("{", formatCode.NoteType.NORMAL, False),
            ("}", formatCode.NoteType.NORMAL, False)
        ]
        formatCode.combine_colon(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_generic_angle_brackets(self):
        input_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("dict", formatCode.NoteType.NORMAL, False),
            ("<", formatCode.NoteType.NORMAL, False),
            ("int", formatCode.NoteType.NORMAL, False),
            (",", formatCode.NoteType.NORMAL, False),
            ("dict", formatCode.NoteType.NORMAL, False),
            ("<", formatCode.NoteType.NORMAL, False),
            ("str", formatCode.NoteType.NORMAL, False),
            (",", formatCode.NoteType.NORMAL, False),
            ("b", formatCode.NoteType.NORMAL, False),
            (">", formatCode.NoteType.NORMAL, False),
            (">", formatCode.NoteType.NORMAL, False),
            ("a", formatCode.NoteType.NORMAL, False),
            ("=", formatCode.NoteType.NORMAL, False),
            ("b", formatCode.NoteType.NORMAL, False),
            (";", formatCode.NoteType.NORMAL, False),
            ("c", formatCode.NoteType.NORMAL, False),
            ("<", formatCode.NoteType.NORMAL, False),
            ("d", formatCode.NoteType.NORMAL, False),
            (";", formatCode.NoteType.NORMAL, False)
        ]
        output_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("dict<int", formatCode.NoteType.NORMAL, False),
            (",", formatCode.NoteType.NORMAL, False),
            ("dict<str", formatCode.NoteType.NORMAL, False),
            (",", formatCode.NoteType.NORMAL, False),
            ("b>>", formatCode.NoteType.NORMAL, False),
            ("a", formatCode.NoteType.NORMAL, False),
            ("=", formatCode.NoteType.NORMAL, False),
            ("b", formatCode.NoteType.NORMAL, False),
            (";", formatCode.NoteType.NORMAL, False),
            ("c", formatCode.NoteType.NORMAL, False),
            ("<", formatCode.NoteType.NORMAL, False),
            ("d", formatCode.NoteType.NORMAL, False),
            (";", formatCode.NoteType.NORMAL, False)
        ]
        formatCode.combine_generic_angle_brackets(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_parentheses(self):
        input_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("if", formatCode.NoteType.NORMAL, False),
            ("(", formatCode.NoteType.NORMAL, False),
            ("b", formatCode.NoteType.NORMAL, False),
            (")", formatCode.NoteType.NORMAL, False),
            ("{", formatCode.NoteType.NORMAL, False),
            ("}", formatCode.NoteType.NORMAL, False),
            (";", formatCode.NoteType.NORMAL, False),
            ("a", formatCode.NoteType.NORMAL, False),
            ("(", formatCode.NoteType.NORMAL, False),
            (")", formatCode.NoteType.NORMAL, False),
            (".", formatCode.NoteType.NORMAL, False),
            ("b", formatCode.NoteType.NORMAL, False),
            ("(", formatCode.NoteType.NORMAL, False),
            ("c", formatCode.NoteType.NORMAL, False),
            (")", formatCode.NoteType.NORMAL, False),
            (";", formatCode.NoteType.NORMAL, False),
            ("a", formatCode.NoteType.NORMAL, False),
            ("(", formatCode.NoteType.NORMAL, False),
            (")", formatCode.NoteType.NORMAL, False),
            ("*", formatCode.NoteType.NORMAL, False),
            ("(", formatCode.NoteType.NORMAL, False),
            ("b", formatCode.NoteType.NORMAL, False),
            (")", formatCode.NoteType.NORMAL, False),
            (";", formatCode.NoteType.NORMAL, False)
        ]
        output_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("if", formatCode.NoteType.NORMAL, False),
            ("(b) {", formatCode.NoteType.NORMAL, False),
            ("}", formatCode.NoteType.NORMAL, False),
            (";", formatCode.NoteType.NORMAL, False),
            ("a().", formatCode.NoteType.NORMAL, False),
            ("b(c);", formatCode.NoteType.NORMAL, False),
            ("a() *", formatCode.NoteType.NORMAL, False),
            ("(b);", formatCode.NoteType.NORMAL, False)
        ]
        formatCode.combine_parentheses(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_square_brackets(self):
        input_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("a", formatCode.NoteType.NORMAL, False),
            ("[", formatCode.NoteType.NORMAL, False),
            ("b", formatCode.NoteType.NORMAL, False),
            ("]", formatCode.NoteType.NORMAL, False),
            ("=", formatCode.NoteType.NORMAL, False),
            ("(c", formatCode.NoteType.NORMAL, False),
            ("[", formatCode.NoteType.NORMAL, False),
            ("d", formatCode.NoteType.NORMAL, False),
            ("]);", formatCode.NoteType.NORMAL, False)
        ]
        output_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("a[b]", formatCode.NoteType.NORMAL, False),
            ("=", formatCode.NoteType.NORMAL, False),
            ("(c[d]);", formatCode.NoteType.NORMAL, False)
        ]
        formatCode.combine_square_brackets(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_semicolon(self):
        input_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("a();", formatCode.NoteType.NORMAL, False),
            ("b", formatCode.NoteType.NORMAL, False),
            ("=", formatCode.NoteType.NORMAL, False),
            ("c", formatCode.NoteType.NORMAL, False),
            (";", formatCode.NoteType.NORMAL, False)
        ]
        output_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("a();", formatCode.NoteType.NORMAL, False),
            ("b", formatCode.NoteType.NORMAL, False),
            ("=", formatCode.NoteType.NORMAL, False),
            ("c;", formatCode.NoteType.NORMAL, False)
        ]
        formatCode.combine_semicolon(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_comma(self):
        input_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("(", formatCode.NoteType.NORMAL, False),
            ("a", formatCode.NoteType.NORMAL, False),
            (")", formatCode.NoteType.NORMAL, False),
            (",", formatCode.NoteType.NORMAL, False),
            ("b", formatCode.NoteType.NORMAL, False),
            (",", formatCode.NoteType.NORMAL, False),
            ("c", formatCode.NoteType.NORMAL, False)
        ]
        output_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("(", formatCode.NoteType.NORMAL, False),
            ("a", formatCode.NoteType.NORMAL, False),
            ("),", formatCode.NoteType.NORMAL, False),
            ("b,", formatCode.NoteType.NORMAL, False),
            ("c", formatCode.NoteType.NORMAL, False)
        ]
        formatCode.combine_comma(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_operator(self):
        input_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("a", formatCode.NoteType.NORMAL, False),
            ("++;", formatCode.NoteType.NORMAL, False),
            ("c", formatCode.NoteType.NORMAL, False),
            ("--;", formatCode.NoteType.NORMAL, False),
            ("d", formatCode.NoteType.NORMAL, False),
            ("=", formatCode.NoteType.NORMAL, False),
            ("-", formatCode.NoteType.NORMAL, False),
            ("e;", formatCode.NoteType.NORMAL, False),
            ("f", formatCode.NoteType.NORMAL, False),
            ("=", formatCode.NoteType.NORMAL, False),
            ("f", formatCode.NoteType.NORMAL, False),
            ("-", formatCode.NoteType.NORMAL, False),
            ("g;", formatCode.NoteType.NORMAL, False)
        ]
        output_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("a++;", formatCode.NoteType.NORMAL, False),
            ("c--;", formatCode.NoteType.NORMAL, False),
            ("d", formatCode.NoteType.NORMAL, False),
            ("=", formatCode.NoteType.NORMAL, False),
            ("-e;", formatCode.NoteType.NORMAL, False),
            ("f", formatCode.NoteType.NORMAL, False),
            ("=", formatCode.NoteType.NORMAL, False),
            ("f", formatCode.NoteType.NORMAL, False),
            ("-", formatCode.NoteType.NORMAL, False),
            ("g;", formatCode.NoteType.NORMAL, False)
        ]
        formatCode.combine_operator(input_code)
        self.assertEqual(input_code, output_code)

    def test_find_array_comma(self):
        input_code: list[tuple[str, formatCode.NoteType]] = [
            ("{", formatCode.NoteType.NORMAL),
            ("a,", formatCode.NoteType.NORMAL),
            ("b", formatCode.NoteType.NORMAL),
            ("}", formatCode.NoteType.NORMAL)
        ]
        calculate_output_code: list[tuple[str, formatCode.NoteType, bool]] = formatCode.find_array_comma(input_code)
        self.assertTrue(calculate_output_code[1][2])
        self.assertEqual(len(calculate_output_code), len(input_code))
        input_code: list[tuple[str, formatCode.NoteType]] = [
            ("{", formatCode.NoteType.NORMAL),
            ("a;", formatCode.NoteType.NORMAL),
            ("f(b,", formatCode.NoteType.NORMAL),
            ("c)", formatCode.NoteType.NORMAL),
            ("{", formatCode.NoteType.NORMAL),
            ("a,", formatCode.NoteType.NORMAL),
            ("b", formatCode.NoteType.NORMAL),
            ("}", formatCode.NoteType.NORMAL),
            ("}", formatCode.NoteType.NORMAL)
        ]
        calculate_output_code: list[tuple[str, formatCode.NoteType, bool]] = formatCode.find_array_comma(input_code)
        self.assertFalse(calculate_output_code[1][2])
        self.assertFalse(calculate_output_code[2][2])
        self.assertFalse(calculate_output_code[3][2])
        self.assertTrue(calculate_output_code[5][2])
        self.assertEqual(len(calculate_output_code), len(input_code))
        input_code: list[tuple[str, formatCode.NoteType]] = [
            ("{", formatCode.NoteType.NORMAL),
            ("a", formatCode.NoteType.NORMAL),
            ("(", formatCode.NoteType.NORMAL),
            ("0", formatCode.NoteType.NORMAL),
            (",", formatCode.NoteType.NORMAL),
            ("0)", formatCode.NoteType.NORMAL),
            ("};", formatCode.NoteType.NORMAL)
        ]
        calculate_output_code: list[tuple[str, formatCode.NoteType, bool]] = formatCode.find_array_comma(input_code)
        self.assertFalse(calculate_output_code[1][2])

    def test_combine_to_line(self):
        input_code: list[tuple[str, formatCode.NoteType, bool]] = [
            ("int", formatCode.NoteType.NORMAL, False),
            ("c", formatCode.NoteType.NORMAL, False),
            ("=", formatCode.NoteType.NORMAL, False),
            ("a;", formatCode.NoteType.NORMAL, False),
            ("/// abc", formatCode.NoteType.NOTE, False),
            ("void", formatCode.NoteType.NORMAL, False),
            ("f()", formatCode.NoteType.NORMAL, False),
            ("{", formatCode.NoteType.NORMAL, False),
            ("// abcd", formatCode.NoteType.NOTE_END, False),
            ("return;", formatCode.NoteType.NORMAL, False),
            ("}", formatCode.NoteType.NORMAL, False),
            ("{", formatCode.NoteType.NORMAL, False),
            ("\"ui\",", formatCode.NoteType.NORMAL, False),
            ("new", formatCode.NoteType.NORMAL, False),
            ("Dictionary", formatCode.NoteType.NORMAL, False),
            ("{", formatCode.NoteType.NORMAL, False),
            ("\"name\",", formatCode.NoteType.NORMAL, False),
            ("\"uiType\"", formatCode.NoteType.NORMAL, False),
            ("},", formatCode.NoteType.NORMAL, False),
            ("}", formatCode.NoteType.NORMAL, False),
            ("public", formatCode.NoteType.NORMAL, False)
        ]
        output_code: list[tuple[str, int]] = [
            ("int c = a;", 0),
            ("/// abc", 0),
            ("void f() { // abcd", 0),
            ("return;", 1),
            ("}", 0),
            ("{", 0),
            ("\"ui\", new Dictionary {", 1),
            ("\"name\", \"uiType\"", 2),
            ("},", 1),
            ("}", 0),
            ("public", 0)
        ]
        calculate_output_code: list[tuple[str, int]] = formatCode.combine_to_line(input_code)
        self.assertEqual(calculate_output_code, output_code)

    def test_combine_for_loop(self):
        input_code: list[tuple[str, int]] = [
            ("for (int i = 0;", 0),
            ("i < 10;", 0),
            ("i++) {", 0),
            ("forWhat();", 1),
            ("}", 0)
        ]
        output_code: list[tuple[str, int]] = [
            ("for (int i = 0; i < 10; i++) {", 0),
            ("forWhat();", 1),
            ("}", 0)
        ]
        formatCode.combine_for_loop(input_code)
        self.assertEqual(input_code, output_code)

    def test_package(self):
        self.assertEqual(formatCode.package("aaa"), ("aaa", formatCode.NoteType.NORMAL))
        self.assertEqual(formatCode.package("abc"), ("abc", formatCode.NoteType.NORMAL))

if __name__ == '__main__':
    unittest.main()
