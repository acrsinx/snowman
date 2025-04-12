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

    def test_package(self):
        self.assertEqual(formatCode.package("aaa"), ("aaa", formatCode.NoteType.NORMAL))
        self.assertEqual(formatCode.package("abc"), ("abc", formatCode.NoteType.NORMAL))

if __name__ == '__main__':
    unittest.main()
