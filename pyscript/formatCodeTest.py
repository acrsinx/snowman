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

    def test_find_and_combine_ternary_operator(self):
        input_code: list[tuple[str, formatCode.NoteType]] = [
            ("a", formatCode.NoteType.NORMAL),
            ("?", formatCode.NoteType.NORMAL),
            ("b", formatCode.NoteType.NORMAL),
            (":", formatCode.NoteType.NORMAL),
            ("c", formatCode.NoteType.NORMAL),
            (";", formatCode.NoteType.NORMAL),
            ("d", formatCode.NoteType.NORMAL),
            (":", formatCode.NoteType.NORMAL)
        ]
        output_code: list[tuple[str, formatCode.NoteType]] = [
            ("a?b:c", formatCode.NoteType.NORMAL),
            (";", formatCode.NoteType.NORMAL),
            ("d", formatCode.NoteType.NORMAL),
            (":", formatCode.NoteType.NORMAL)
        ]
        formatCode.find_and_combine_ternary_operator(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_colon(self):
        input_code: list[tuple[str, formatCode.NoteType]] = [
            ("class", formatCode.NoteType.NORMAL),
            ("a", formatCode.NoteType.NORMAL),
            (":", formatCode.NoteType.NORMAL),
            ("{", formatCode.NoteType.NORMAL),
            ("}", formatCode.NoteType.NORMAL)
        ]
        output_code: list[tuple[str, formatCode.NoteType]] = [
            ("class", formatCode.NoteType.NORMAL),
            ("a:", formatCode.NoteType.NORMAL),
            ("{", formatCode.NoteType.NORMAL),
            ("}", formatCode.NoteType.NORMAL)
        ]
        formatCode.combine_colon(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_generic_angle_brackets(self):
        input_code: list[tuple[str, formatCode.NoteType]] = [
            ("dict", formatCode.NoteType.NORMAL),
            ("<", formatCode.NoteType.NORMAL),
            ("int", formatCode.NoteType.NORMAL),
            (",", formatCode.NoteType.NORMAL),
            ("dict", formatCode.NoteType.NORMAL),
            ("<", formatCode.NoteType.NORMAL),
            ("str", formatCode.NoteType.NORMAL),
            (",", formatCode.NoteType.NORMAL),
            ("b", formatCode.NoteType.NORMAL),
            (">", formatCode.NoteType.NORMAL),
            (">", formatCode.NoteType.NORMAL),
            ("a", formatCode.NoteType.NORMAL),
            ("=", formatCode.NoteType.NORMAL),
            ("b", formatCode.NoteType.NORMAL),
            (";", formatCode.NoteType.NORMAL),
            ("c", formatCode.NoteType.NORMAL),
            ("<", formatCode.NoteType.NORMAL),
            ("d", formatCode.NoteType.NORMAL),
            (";", formatCode.NoteType.NORMAL)
        ]
        output_code: list[tuple[str, formatCode.NoteType]] = [
            ("dict<int", formatCode.NoteType.NORMAL),
            (",", formatCode.NoteType.NORMAL),
            ("dict<str", formatCode.NoteType.NORMAL),
            (",", formatCode.NoteType.NORMAL),
            ("b>>", formatCode.NoteType.NORMAL),
            ("a", formatCode.NoteType.NORMAL),
            ("=", formatCode.NoteType.NORMAL),
            ("b", formatCode.NoteType.NORMAL),
            (";", formatCode.NoteType.NORMAL),
            ("c", formatCode.NoteType.NORMAL),
            ("<", formatCode.NoteType.NORMAL),
            ("d", formatCode.NoteType.NORMAL),
            (";", formatCode.NoteType.NORMAL)
        ]
        formatCode.combine_generic_angle_brackets(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_parentheses(self):
        input_code: list[tuple[str, formatCode.NoteType]] = [
            ("if", formatCode.NoteType.NORMAL),
            ("(", formatCode.NoteType.NORMAL),
            ("b", formatCode.NoteType.NORMAL),
            (")", formatCode.NoteType.NORMAL),
            ("{", formatCode.NoteType.NORMAL),
            ("}", formatCode.NoteType.NORMAL),
            (";", formatCode.NoteType.NORMAL),
            ("a", formatCode.NoteType.NORMAL),
            ("(", formatCode.NoteType.NORMAL),
            (")", formatCode.NoteType.NORMAL),
            (".", formatCode.NoteType.NORMAL),
            ("b", formatCode.NoteType.NORMAL),
            ("(", formatCode.NoteType.NORMAL),
            ("c", formatCode.NoteType.NORMAL),
            (")", formatCode.NoteType.NORMAL),
            (";", formatCode.NoteType.NORMAL),
            ("a", formatCode.NoteType.NORMAL),
            ("(", formatCode.NoteType.NORMAL),
            (")", formatCode.NoteType.NORMAL),
            ("*", formatCode.NoteType.NORMAL),
            ("(", formatCode.NoteType.NORMAL),
            ("b", formatCode.NoteType.NORMAL),
            (")", formatCode.NoteType.NORMAL),
            (";", formatCode.NoteType.NORMAL)
        ]
        output_code: list[tuple[str, formatCode.NoteType]] = [
            ("if", formatCode.NoteType.NORMAL),
            ("(b) {", formatCode.NoteType.NORMAL),
            ("}", formatCode.NoteType.NORMAL),
            (";", formatCode.NoteType.NORMAL),
            ("a().", formatCode.NoteType.NORMAL),
            ("b(c);", formatCode.NoteType.NORMAL),
            ("a() *", formatCode.NoteType.NORMAL),
            ("(b);", formatCode.NoteType.NORMAL)
        ]
        formatCode.combine_parentheses(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_square_brackets(self):
        input_code: list[tuple[str, formatCode.NoteType]] = [
            ("a", formatCode.NoteType.NORMAL),
            ("[", formatCode.NoteType.NORMAL),
            ("b", formatCode.NoteType.NORMAL),
            ("]", formatCode.NoteType.NORMAL),
            ("=", formatCode.NoteType.NORMAL),
            ("(c", formatCode.NoteType.NORMAL),
            ("[", formatCode.NoteType.NORMAL),
            ("d", formatCode.NoteType.NORMAL),
            ("]);", formatCode.NoteType.NORMAL)
        ]
        output_code: list[tuple[str, formatCode.NoteType]] = [
            ("a[b]", formatCode.NoteType.NORMAL),
            ("=", formatCode.NoteType.NORMAL),
            ("(c[d]);", formatCode.NoteType.NORMAL)
        ]
        formatCode.combine_square_brackets(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_semicolon(self):
        input_code: list[tuple[str, formatCode.NoteType]] = [
            ("a();", formatCode.NoteType.NORMAL),
            ("b", formatCode.NoteType.NORMAL),
            ("=", formatCode.NoteType.NORMAL),
            ("c", formatCode.NoteType.NORMAL),
            (";", formatCode.NoteType.NORMAL)
        ]
        output_code: list[tuple[str, formatCode.NoteType]] = [
            ("a();", formatCode.NoteType.NORMAL),
            ("b", formatCode.NoteType.NORMAL),
            ("=", formatCode.NoteType.NORMAL),
            ("c;", formatCode.NoteType.NORMAL)
        ]
        formatCode.combine_semicolon(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_comma(self):
        input_code: list[tuple[str, formatCode.NoteType]] = [
            ("(", formatCode.NoteType.NORMAL),
            ("a", formatCode.NoteType.NORMAL),
            (")", formatCode.NoteType.NORMAL),
            (",", formatCode.NoteType.NORMAL),
            ("b", formatCode.NoteType.NORMAL),
            (",", formatCode.NoteType.NORMAL),
            ("c", formatCode.NoteType.NORMAL)
        ]
        output_code: list[tuple[str, formatCode.NoteType]] = [
            ("(", formatCode.NoteType.NORMAL),
            ("a", formatCode.NoteType.NORMAL),
            ("),", formatCode.NoteType.NORMAL),
            ("b,", formatCode.NoteType.NORMAL),
            ("c", formatCode.NoteType.NORMAL)
        ]
        formatCode.combine_comma(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_operator(self):
        input_code: list[tuple[str, formatCode.NoteType]] = [
            ("a", formatCode.NoteType.NORMAL),
            ("++;", formatCode.NoteType.NORMAL),
            ("c", formatCode.NoteType.NORMAL),
            ("--;", formatCode.NoteType.NORMAL),
            ("d", formatCode.NoteType.NORMAL),
            ("=", formatCode.NoteType.NORMAL),
            ("-", formatCode.NoteType.NORMAL),
            ("e;", formatCode.NoteType.NORMAL),
            ("f", formatCode.NoteType.NORMAL),
            ("=", formatCode.NoteType.NORMAL),
            ("f", formatCode.NoteType.NORMAL),
            ("-", formatCode.NoteType.NORMAL),
            ("g;", formatCode.NoteType.NORMAL)
        ]
        output_code: list[tuple[str, formatCode.NoteType]] = [
            ("a++;", formatCode.NoteType.NORMAL),
            ("c--;", formatCode.NoteType.NORMAL),
            ("d", formatCode.NoteType.NORMAL),
            ("=", formatCode.NoteType.NORMAL),
            ("-e;", formatCode.NoteType.NORMAL),
            ("f", formatCode.NoteType.NORMAL),
            ("=", formatCode.NoteType.NORMAL),
            ("f", formatCode.NoteType.NORMAL),
            ("-", formatCode.NoteType.NORMAL),
            ("g;", formatCode.NoteType.NORMAL)
        ]
        formatCode.combine_operator(input_code)
        self.assertEqual(input_code, output_code)

    def test_combine_to_line(self):
        input_code: list[tuple[str, formatCode.NoteType]] = [
            ("int", formatCode.NoteType.NORMAL),
            ("c", formatCode.NoteType.NORMAL),
            ("=", formatCode.NoteType.NORMAL),
            ("a;", formatCode.NoteType.NORMAL),
            ("/// abc", formatCode.NoteType.NOTE),
            ("void", formatCode.NoteType.NORMAL),
            ("f()", formatCode.NoteType.NORMAL),
            ("{", formatCode.NoteType.NORMAL),
            ("// abcd", formatCode.NoteType.NOTE_END),
            ("return;", formatCode.NoteType.NORMAL),
            ("}", formatCode.NoteType.NORMAL),
            ("{", formatCode.NoteType.NORMAL),
            ("\"ui\",", formatCode.NoteType.NORMAL),
            ("new", formatCode.NoteType.NORMAL),
            ("Dictionary", formatCode.NoteType.NORMAL),
            ("{", formatCode.NoteType.NORMAL),
            ("\"name\",", formatCode.NoteType.NORMAL),
            ("\"uiType\"", formatCode.NoteType.NORMAL),
            ("},", formatCode.NoteType.NORMAL),
            ("}", formatCode.NoteType.NORMAL),
            ("public", formatCode.NoteType.NORMAL)
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
