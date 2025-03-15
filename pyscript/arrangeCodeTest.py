import unittest
import arrangeCode

class TestArrangeCode(unittest.TestCase):
    def test_is_operator(self):
        self.assertTrue(arrangeCode.is_operator("+"))
        self.assertTrue(arrangeCode.is_operator("-"))
        self.assertTrue(arrangeCode.is_operator("*"))
        self.assertTrue(arrangeCode.is_operator("/"))
        self.assertFalse(arrangeCode.is_operator("a"))
        self.assertFalse(arrangeCode.is_operator("1"))
        self.assertFalse(arrangeCode.is_operator(" "))
        self.assertFalse(arrangeCode.is_operator("()"))

    def test_package(self):
        self.assertEqual(arrangeCode.package("aaa"), ("aaa", arrangeCode.NoteType.NORMAL))
        self.assertEqual(arrangeCode.package("abc"), ("abc", arrangeCode.NoteType.NORMAL))

if __name__ == '__main__':
    unittest.main()
