import unittest

import makePlotJson

class TestMakePlotJson(unittest.TestCase):
	def test_decode_mdplot(self):
		mdplot = makePlotJson.decode_mdplot("```a```bcd`e`f`g`")
		self.assertEqual(mdplot, ["a", "e", "g"])

if __name__ == '__main__':
	unittest.main()
