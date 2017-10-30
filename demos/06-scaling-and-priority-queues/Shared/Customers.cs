using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared
{
    public static class Customers
    {
        public static IEnumerable<Guid> GetPriorityCustomers()
        {
            return new List<Guid>
            {
                new Guid("07b336c4-641e-4fa2-9cde-55b58de82161"),
                new Guid("68cac21d-185c-4bba-a99f-96f197120a14"),
                new Guid("7ddeb593-67ca-4b16-9f03-c81e5d17831d"),
                new Guid("348b7416-2ced-47ec-bf67-4bdfe73188eb"),
                new Guid("f7c5482a-815f-496e-ad4b-0f3a42b4aa4d")
            };
        }

        public static IEnumerable<Guid> GetAllCustomers()
        {
            return GetPriorityCustomers().Concat(new List<Guid>
                {
                new Guid("b547d620-fdc5-4401-a627-3dfa8b286b38"),
                new Guid("729898ab-8176-4e94-b3e5-95794b34ac2f"),
                new Guid("c596e00d-8eb6-4fcb-bb15-9ad322984504"),
                new Guid("a72b9347-9165-47b0-bf5c-74160156118c"),
                new Guid("532f265b-a0a5-40b2-a4b0-22e463725d32"),
                new Guid("823d5477-6d41-4335-bd8f-e6cbe0f9c331"),
                new Guid("b9c7a96d-1ac8-4c5a-acba-f0bb5f7ac504"),
                new Guid("ec8081c2-03c6-46a8-85da-d7bef2817e16"),
                new Guid("4393018f-66c2-42e8-8a04-6e4b8e5cfa26"),
                new Guid("76ab89c4-c6ac-4775-88e6-97e8f810b2f5"),
                new Guid("8c49e9f5-2b37-45bb-8e8c-614bef7c0a0f"),
                new Guid("f2417749-3586-4b1c-8359-623b88ebc643"),
                new Guid("6934bda6-4c89-461e-9b91-dff061be4170"),
                new Guid("9e846dad-7a70-4551-95f5-358bc7751d67"),
                new Guid("8818ca7c-c7d5-4596-b239-45a5abbd0e74")
            });
        }
    }
}
