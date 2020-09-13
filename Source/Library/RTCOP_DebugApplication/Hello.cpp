#include "Hello.h"
#include <stdio.h>

namespace baselayer
{
	// ベースクラスHelloの実装
	void Hello::Print()
	{
		printf("Print: Base_Hello\n");
	}

	void Hello::Print2(char arg)
	{
		this->_BaseMember = arg;
		printf("Print2: Base_Hello %c\n", this->_BaseMember);
	}

	void Hello::Print3()
	{
		printf("Print: Base_Hello\n");
	}

	Hello::Hello(int a)
	{

	}
}
