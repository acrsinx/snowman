import unittest

import main
import makeTranslate

class TestMakeTranslate(unittest.TestCase):
    def test_plot_json_path_to_template_path(self):
        main.check_current_directory()
        self.assertEqual(makeTranslate.plot_json_path_to_template_path("plotJson\\plot0\\plot0_0.json"), "localization\\template\\plot\\plot0_plot0_0.md")

    def test_localization_path_to_template_path(self):
        self.assertEqual(makeTranslate.localization_path_to_template_path("localization\\en\\plot\\plot0_plot0_0.md"), "localization\\template\\plot\\plot0_plot0_0.md")
        self.assertEqual(makeTranslate.localization_path_to_template_path("localization\\zh\\core.md"), "localization\\template\\core.md")

if __name__ == '__main__':
    unittest.main()
