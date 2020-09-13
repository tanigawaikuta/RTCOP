#ifndef __HELLO_H__
#define __HELLO_H__

namespace baselayer
{
	// ベースクラスHelloの定義
	class Hello
	{
	private:
		char _BaseMember;
	public:
		virtual void Print();
		virtual void Print2(char arg);
		virtual void Print3();
		Hello(int a);
	};
}

#endif
