﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace YouTrack.Rest.Features.General.Issues
{
    [Binding]
    [Scope(Feature = "Get Comments of an Issue")]
    public class GetCommentsOfAnIssueSteps : IssueSteps
    {
        private string GetIssueId()
        {
            return ScenarioContext.Current.Get<string>("issueId");
        }

        [Given(@"I have created an issue")]
        public void GivenIHaveCreatedAnIssue()
        {
            string issueId = StepHelper.CreateIssue("SB", "Testing", "Fetching Comments");

            ScenarioContext.Current.Set(issueId, "issueId");
        }

        [Given(@"I have created an comment to the issue")]
        public void GivenIHaveCreatedAnCommentToTheIssue()
        {
            StepHelper.AddCommentToIssue(GetIssueId(), "testing");
        }

        [When(@"I fetch the comments for the issue")]
        public void WhenIFetchTheCommentsForTheIssue()
        {
            IEnumerable<IComment> comments = StepHelper.GetComments(GetIssueId());

            ScenarioContext.Current.Set(comments);
        }

        [Then(@"I receive one comment")]
        public void ThenIReceiveOneComment()
        {
            Assert.That(ScenarioContext.Current.Get<IEnumerable<IComment>>().Count(), Is.EqualTo(1));
        }

        [Given(@"I have created two comments to the issue")]
        public void GivenIHaveCreatedTwoCommentsToTheIssue()
        {
            StepHelper.AddCommentToIssue(GetIssueId(), "comment 1");
            StepHelper.AddCommentToIssue(GetIssueId(), "comment 2");
        }

        [Then(@"I receive two comments")]
        public void ThenIReceiveTwoComments()
        {
            Assert.That(ScenarioContext.Current.Get<IEnumerable<IComment>>().Count(), Is.EqualTo(2));
        }

    }
}
